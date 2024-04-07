using System.Linq.Expressions;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter"/> that chains
/// all passed in child SearchFilters in logical AND operators
/// </summary>
public class SearchFilterAnd : SearchFilter
{
    /// <summary>
    /// Discriminator for this <see cref="SearchFilter"/>
    /// </summary>
    public required List<SearchFilter> And { get; init; }

    /// <inheritdoc/>
    public override Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        return And
            .Select(o => o.Compile(ctx))
            .Aggregate(Expression.And);
    }
}
