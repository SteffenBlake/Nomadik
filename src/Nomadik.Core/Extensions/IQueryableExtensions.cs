using Nomadik.Core;

namespace Nomadik.Extensions;

/// <summary>
/// Extends <see cref="IQueryable{T}"/>
/// </summary>
public static class IQueryableExtensions
{
    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.SearchAsync(IQueryable{T})" />
    public static Task<SearchQueryResult<TOut>> SearchAsync<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.SearchAsync(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.Where(IQueryable{T})" />
    public static IQueryable<TIn> Where<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Where(data);
    }


    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.TryOrderBy(IQueryable{T})" />
    public static IQueryable<TIn> TryOrderBy<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.TryOrderBy(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.OrderBy(IQueryable{T})" />
    public static IQueryable<TIn> OrderBy<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.OrderBy(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.TryThenBy(IOrderedQueryable{T})" />
    public static IQueryable<TIn> TryThenBy<TIn, TOut>(
        this IOrderedQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.TryThenBy(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.ThenBy(IOrderedQueryable{T})" />
    public static IQueryable<TIn> ThenBy<TIn, TOut>(
        this IOrderedQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.ThenBy(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.Page(IQueryable{T})" />
    public static IQueryable<TIn> Page<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Page(data);
    }

    /// <inheritdoc cref="CompiledSearchQuery{TIn, TOut}.Select(IQueryable{T})" />
    public static IQueryable<TOut> Select<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Select(data);
    }
}
