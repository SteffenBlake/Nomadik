using System.Linq.Expressions;
using Nomadik.Core.Abstractions;

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
    public override Expression Compile<TIn, TOut>(INomadik<TIn, TOut> context)
    {
        return And
            .Select(o => o.Compile(context))
            .Aggregate(Expression.And);
    }
}
