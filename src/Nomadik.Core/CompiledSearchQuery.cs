using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nomadik.Core.Extensions;

namespace Nomadik.Core;

/// <summary>
/// Represents a <see cref="SearchQuery"/> that has been compiled against
/// a given Expression InitMember Mapper, binding it to a hard type
/// and caching the Expression Mappings for compiling Filter/Search Queries
/// </summary>
public class CompiledSearchQuery<TIn, TOut>(
    SearchQuery query,
    Expression<Func<TIn, TOut>> mapper
)
{
    private SearchQuery Query { get; } = query;

    private Expression<Func<TIn, TOut>> Mapper { get; } = mapper;

    private IReadOnlyDictionary<string, Expression> Table { get; }
        = mapper.ToMemberTable();

    private IReadOnlyCollection<ParameterExpression> Parameters { get; }
        = mapper.Parameters;

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

        var dataTask = Select(paged).ToListAsync();

        // We can avoid querying the total count of the db
        // if there wasn't pagination, as the count of the results
        // will also be the count of the total possible results
        // however if pagination is enabled then that inherently requires
        // a second Count query on the db
        var ofTask = Query.Filter == null ?
            dataTask.ContinueWith(d => d.Result.Count) :
            filtered.CountAsync();

        await Task.WhenAll(ofTask, dataTask);

        var from = (Query.Page?.Skip ?? 0) + 1;

        return new (
            dataTask.Result, 
            from,
            ofTask.Result
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
        if (Query.Filter == null)
        {
            return data;
        }

        var filterExpression = Query.Filter.Compile(Table);
        var filterLambda = Expression.Lambda<Func<TIn, bool>>(
            filterExpression, Parameters
        );

        return data.Where(filterLambda);
    }

    /// <summary>
    /// Null safe version of <see cref="OrderBy(IQueryable{TIn})"/>. 
    /// Uses the bound <see cref="Query"/>.<see cref="SearchQuery.Order"/> to sort
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. If <see cref="SearchQuery.Order"/> is
    /// null, returns the original unmodified data.
    /// </summary>
    public IQueryable<TIn> TryOrderBy(
        IQueryable<TIn> data 
    )
    {
        if (Query.Order == null)
        {
            return data;
        }

        return OrderByInternal(
            data, 
            Query.Order
        );
    }

    /// <summary>
    /// Uses the bound <see cref="Query"/>.<see cref="SearchQuery.Order"/> to sort
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. Throws a <see cref="NullReferenceException"/>
    /// if <see cref="Query"/>.<see cref="SearchQuery.Order"/> is null
    /// </summary>
    public IOrderedQueryable<TIn> OrderBy(
        IQueryable<TIn> data 
    )
    {
        return OrderByInternal(
            data, 
            Query.Order ?? throw new NullReferenceException(
                $"{nameof(Query)}.{nameof(SearchQuery.Order)} is required for ThenBy()"
            )
        );
    }

    private IOrderedQueryable<TIn> OrderByInternal(
        IQueryable<TIn> data,
        SearchOrder order
    )
    {
        var orderLambda = order.Compile<TIn>(Table, Parameters);

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
    /// Uses the bound <see cref="Query"/>.<see cref="SearchQuery.Order"/> 
    /// to further sort a mapped <see cref="IOrderedQueryable{T}"/> 
    /// of matching bound <typeparamref name="TIn"/>.
    /// </summary>
    public IOrderedQueryable<TIn> TryThenBy(
        IOrderedQueryable<TIn> data 
    )
    {
        if (Query.Order == null)
        {
            return data;
        }

        return ThenByInternal(
            data, 
            Query.Order
        );
    }

    /// <summary>
    /// Uses the bound <see cref="Query"/>.<see cref="SearchQuery.Order"/> to further sort
    /// a mapped <see cref="IOrderedQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. Throws a <see cref="NullReferenceException"/>
    /// if <see cref="Query"/>.<see cref="SearchQuery.Order"/> is null
    /// </summary>
    public IOrderedQueryable<TIn> ThenBy(
        IOrderedQueryable<TIn> data 
    )
    {
        return ThenByInternal(
            data, 
            Query.Order ?? throw new NullReferenceException(
                $"{nameof(Query)}.{nameof(SearchQuery.Order)} is required for ThenBy()"
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
            var orderLambda = target.Compile<TIn>(Table, Parameters); 

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
        if (Query.Page == null)
        {
            return data;
        }

        if (Query.Page.Num == 1)
        {
            return data.Take(Query.Page.Size);
        }

        return data
            .Skip(Query.Page.Skip)
            .Take(Query.Page.Take);
    }

    /// <summary>
    /// Leverages a Search's bound MemberInit Expression to perform a
    /// LINQ Select Operation on a Query
    /// </summary>
    public IQueryable<TOut> Select(
        IQueryable<TIn> data
    )
    {
        return data.Select(Mapper);
    }
}
