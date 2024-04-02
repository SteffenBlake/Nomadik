using System.Linq.Expressions;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter{T}"/> that chains
/// All passed in child SearchFilters in logical OR operators
/// </summary>
public class SearchFilterOr : SearchFilter
{
    public required List<SearchFilter> Or { get; init; }
    /// <inheritdoc/>
    public override Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        return Or
            .Select(o => o.Compile(ctx))
            .Aggregate(Expression.Or);
    }
}
