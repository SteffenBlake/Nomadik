using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Converters;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining a complex search query function
/// </summary>
public class SearchQuery
{
    [JsonConverter(typeof(SearchFilterConverter))]
    public SearchFilter? Filter { get; init; } = null;

    public SearchOrder? Order { get; init; } = null;

    public SearchPagination? Page { get; init; } = null;
  
    /// <summary>
    /// Compiles this <see cref="SearchQuery"/> against a
    /// Hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        Expression<Func<TIn, TOut>> mapper
    ) => new (this, mapper);
}
