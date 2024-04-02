using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nomadik.Core.Extensions;

namespace Nomadik.Core;

/// <summary>
/// Represents a <see cref="SearchQuery"/> that has been compiled against
/// A given Expression InitMember Mapper, binding it to a hard type
/// And caching the Expression Mappings for compiling Filter/Search Queries
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
    /// <see cref="Where(IQueryable{TIn})"/>,
    /// then 
    /// <see cref="Orderby(IQueryable{TIn})"/>,
    /// and finally
    /// <see cref="Page(IQueryable{TIn})"/>
    /// back to back.
    /// If Pagination is enabled, the total count of unpaged items is
    /// simultaneously async queried to populate the 
    /// <see cref="SearchQueryResult{T}.Of"/> Property.
    /// </summary>
    public async Task<SearchQueryResult<TOut>> SearchAsync(
        IQueryable<TIn> data
    )
    {
        var filtered = Where(data);

        var ordered = OrderBy(filtered);
        var paged = Page(ordered);

        var dataTask = Select(paged).ToListAsync();

        // We can avoid querying the total count of the db
        // If there wasnt pagination, as the count of the results
        // Will also be the count of the total possible results
        // However if pagination is enabled then that inherently requires
        // A second Count query on the db
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
    /// Uses the bound <see cref="SearchQuery.Order"/> to sort
    /// a mapped <see cref="IQueryable{T}"/> of matching bound 
    /// <typeparamref name="TIn"/>. If the Order is null it will
    /// just return the same unmodified <paramref name="data"/>
    /// </summary>
    public IQueryable<TIn> OrderBy(
        IQueryable<TIn> data 
    )
    {
        if (Query.Order == null)
        {
            return data;
        }

        var orderExpression = Query.Order.Compile(Table);
        var orderLambda = Expression.Lambda<Func<TIn, object>>(
            orderExpression, Parameters
        );

        return Query.Order.Dir switch 
        {
            OrderDir.Asc => data.OrderBy(orderLambda),
            OrderDir.Desc => data.OrderByDescending(orderLambda),
            _ => throw new NotImplementedException()
        };
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
