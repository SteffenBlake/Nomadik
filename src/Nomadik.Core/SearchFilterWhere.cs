using System.Linq.Expressions;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <summary>
/// "Leaf" Implementation of <see cref="SearchFilter"/>
/// that terminates the recursive chain on a <see cref="SearchOperation"/>
/// </summary>
public class SearchFilterWhere : SearchFilter
{
    /// <summary>
    /// Discriminator for this <see cref="SearchFilter"/>
    /// </summary>
    public required SearchOperation Where { get; init; }

    /// <inheritdoc/>
    public override Expression Compile<TIn, TOut>(INomadik<TIn, TOut> context)
    {
        return Where.Compile(context);
    }
}
