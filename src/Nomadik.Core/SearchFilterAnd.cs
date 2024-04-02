using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Converters;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter{T}"/> that chains
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
