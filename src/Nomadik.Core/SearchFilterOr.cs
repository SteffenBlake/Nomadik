using System.Linq.Expressions;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter"/> that chains
/// all passed in child SearchFilters in logical OR operators
/// </summary>
public class SearchFilterOr : SearchFilter
{
    /// <summary>
    /// Discriminator for this <see cref="SearchFilter"/>
    /// </summary>
    public required List<SearchFilter> Or { get; init; }

    /// <inheritdoc/>
    public override Expression Compile<TIn, TOut>(INomadik<TIn, TOut> context)
    {
        return Or
            .Select(o => o.Compile(context))
            .Aggregate(Expression.Or);
    }
}
