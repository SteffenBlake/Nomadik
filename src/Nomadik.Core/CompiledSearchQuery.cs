using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <summary>
/// Represents a <see cref="SearchQuery"/> that has been compiled against
/// a given <see cref="INomadik{TIn, TOut}/> mapping", binding it to a hard type
/// and caching the Expression Mappings for compiling Filter/Search Queries
/// </summary>
public class CompiledSearchQuery<TIn, TOut>(
    INomadik<TIn, TOut> context,
    SearchQuery query
)
{
    private readonly SearchQuery _query = query;
    private readonly INomadik<TIn, TOut> _context = context;

    /// <summary>
    /// Runs 
    /// <see cref="CompiledSearchQuery{TIn, TOut}.Where(IQueryable{TIn})"/>,
    /// then 
    /// <see cref="CompiledSearchQuery{TIn, TOut}.OrderBy(IQueryable{TIn})"/>,
    /// and finally
    /// <see cref="CompiledSearchQuery{TIn, TOut}.Page(IQueryable{TIn})"/>
    /// back to back.
    /// If Pagination is enabled, the total count of unpaged items is
    /// simultaneously async queried to populate the 
    /// <see cref="SearchQueryResult{T}.Of"/> property.
    /// </summary>
    public async Task<SearchQueryResult<TOut>> SearchAsync(
        IQueryable<TIn> data
    )
    {
        var filtered = Where(data);

        var ordered = TryOrderBy(filtered);
        var paged = Page(ordered);

        var result = await Select(paged).ToListAsync();

        // We can avoid querying the total count of the db
        // if there wasn't pagination, as the count of the results
        // will also be the count of the total possible results
        // however if pagination is enabled then that inherently requires
        // a second Count query on the db
        var of = _query.Filter == null ?
            result.Count :
            await filtered.CountAsync();

        var from = (_query.Page?.Skip ?? 0) + 1;

        return new (
            result, 
            from,
            of
        );
    }

    /// <summary>
    /// Uses the bound <see cref="SearchQuery.Filter"/> to filter
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. If the Filter is null it will
    /// just return the same unmodified <paramref name="data"/>
    /// </summary>
    public IQueryable<TIn> Where(
        IQueryable<TIn> data 
    )
    {
        if (_query.Filter == null)
        {
            return data;
        }

        var filterExpression = _query.Filter.Compile(_context);
        var filterLambda = Expression.Lambda<Func<TIn, bool>>(
            filterExpression, _context.Mapper.Parameters 
        );

        return data.Where(filterLambda);
    }

    /// <summary>
    /// Null safe version of <see cref="OrderBy(IQueryable{TIn})"/>. 
    /// Uses the bound <see cref="SearchQuery.Order"/> to sort
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. If <see cref="SearchQuery.Order"/> is
    /// null, returns the original unmodified data.
    /// </summary>
    public IQueryable<TIn> TryOrderBy(
        IQueryable<TIn> data 
    )
    {
        if (_query.Order == null)
        {
            return data;
        }

        return OrderByInternal(
            data, 
            _query.Order
        );
    }

    /// <summary>
    /// Uses the bound <see cref="SearchQuery.Order"/> to sort
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. Throws a <see cref="NullReferenceException"/>
    /// if <see cref="SearchQuery.Order"/> is null
    /// </summary>
    public IOrderedQueryable<TIn> OrderBy(
        IQueryable<TIn> data 
    )
    {
        return OrderByInternal(
            data, 
            _query.Order ?? throw new NullReferenceException(
                $"{nameof(SearchQuery.Order)} is required for ThenBy()"
            )
        );
    }

    private IOrderedQueryable<TIn> OrderByInternal(
        IQueryable<TIn> data,
        SearchOrder order
    )
    {
        var orderLambda = order.Compile(_context);

        var ordered = order.Dir switch 
        {
            OrderDir.Asc => data.OrderBy(orderLambda),
            OrderDir.Desc => data.OrderByDescending(orderLambda),
            _ => throw new NotImplementedException()
        };

        return order.Then == null ? ordered : ThenByInternal(ordered, order.Then);
    }

    /// <summary>
    /// Null safe version of <see cref="ThenBy(IOrderedQueryable{TIn})"/>.
    /// Uses the bound <see cref="SearchQuery.Order"/> 
    /// to further sort a mapped <see cref="IOrderedQueryable{T}"/> 
    /// of matching bound <typeparamref name="TIn"/>.
    /// If <see cref="SearchQuery.Order"/> is null it will return the original
    /// data unchanged.
    /// </summary>
    public IOrderedQueryable<TIn> TryThenBy(
        IOrderedQueryable<TIn> data 
    )
    {
        if (_query.Order == null)
        {
            return data;
        }

        return ThenByInternal(
            data, 
            _query.Order
        );
    }

    /// <summary>
    /// Uses the bound <see cref="SearchQuery.Order"/> to further sort
    /// a mapped <see cref="IOrderedQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. Throws a <see cref="NullReferenceException"/>
    /// if <see cref="SearchQuery.Order"/> is null
    /// </summary>
    public IOrderedQueryable<TIn> ThenBy(
        IOrderedQueryable<TIn> data 
    )
    {
        return ThenByInternal(
            data, 
            _query.Order ?? throw new NullReferenceException(
                $"{nameof(_query)}.{nameof(SearchQuery.Order)} is required for ThenBy()"
            )
        );
    }

    private IOrderedQueryable<TIn> ThenByInternal(
        IOrderedQueryable<TIn> data,
        SearchOrder order
    )
    {
        // Recursively iterate down then "ThenBy" stack of orderBy's
        var result = data;
        var target = order;
        
        while (target != null)
        {
            var orderLambda = target.Compile(_context); 

            result = target.Dir switch 
            {
                OrderDir.Asc => result.ThenBy(orderLambda),
                OrderDir.Desc => result.ThenByDescending(orderLambda),
                _ => throw new NotImplementedException()
            };

            target = target.Then;
        }

        return result;
    }

    /// <summary>
    /// Uses the bound <see cref="SearchQuery.Page"/> to Paginate
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. If the Page is null it will
    /// just return the same unmodified <paramref name="data"/>
    /// </summary>
    public IQueryable<TIn> Page(
        IQueryable<TIn> data 
    )
    {
        if (_query.Page == null)
        {
            return data;
        }

        if (_query.Page.Num == 1)
        {
            return data.Take(_query.Page.Size);
        }

        return data
            .Skip(_query.Page.Skip)
            .Take(_query.Page.Take);
    }

    /// <summary>
    /// Leverages a <see cref="INomadik{TIn, TOut}" bound MemberInit Expression
    /// to perform a LINQ Select Operation on a Query
    /// </summary>
    public IQueryable<TOut> Select(
        IQueryable<TIn> data
    )
    {
        return data.Select(_context.Mapper);
    }
}
