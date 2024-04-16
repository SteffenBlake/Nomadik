using System.Linq.Expressions;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <summary>
/// Implementation of <see cref="SearchFilter"/> that inverts the
/// logic via a NOT operator of its child 
/// </summary>
public class SearchFilterNot : SearchFilter
{
    /// <summary>
    /// Discriminator for this <see cref="SearchFilter"/>
    /// </summary>
    public required SearchFilter Not { get; init; } 

    /// <inheritdoc/>
    public override Expression Compile<TIn, TOut>(
        INomadik<TIn, TOut> context
    )
    {
        return Expression.Not(Not.Compile(context));
    }
}
