using Microsoft.EntityFrameworkCore;
using Nomadik.Core;

namespace Nomadik.Extensions;

/// <summary>
/// Extends <see cref="IQueryable{T}"/>
/// </summary>
public static class IQueryableExtensions
{
    /// <inheritdoc 
    ///     cref="CompiledSearchQuery{TIn, TOut}.SearchAsync(IQueryable{TIn})" 
    /// />
    public static Task<SearchQueryResult<TOut>> SearchAsync<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.SearchAsync(data);
    }

    /// <inheritdoc 
    ///     cref="CompiledSearchQuery{TIn, TOut}.Where(IQueryable{TIn})" 
    /// />
    public static IQueryable<TIn> Where<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Where(data);
    }

    /// <inheritdoc 
    ///     cref="CompiledSearchQuery{TIn, TOut}.OrderBy(IQueryable{TIn})" 
    /// />
    public static IQueryable<TIn> OrderBy<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.OrderBy(data);
    }

    /// <inheritdoc 
    ///     cref="CompiledSearchQuery{TIn, TOut}.Page(IQueryable{TIn})" 
    /// />
    public static IQueryable<TIn> Page<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Page(data);
    }

    /// <inheritdoc 
    ///     cref="CompiledSearchQuery{TIn, TOut}.Select(IQueryable{TIn})" 
    /// />
    public static IQueryable<TOut> Select<TIn, TOut>(
        this IQueryable<TIn> data, 
        CompiledSearchQuery<TIn, TOut> query
    )
    {
        return query.Select(data);
    }
}
