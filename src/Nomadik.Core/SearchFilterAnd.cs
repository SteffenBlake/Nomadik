using System.Linq.Expressions;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter"/> that chains
/// All passed in child SearchFilters in logical AND operators
/// </summary>
public class SearchFilterAnd : SearchFilter
{
    public required List<SearchFilter> And { get; init; }

    /// <inheritdoc/>
    public override Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        return And
            .Select(o => o.Compile(ctx))
            .Aggregate(Expression.And);
    }
}
