using System.Linq.Expressions;

namespace Nomadik.Core;

/// <summary>
/// "Leaf" Implementation of <see cref="SearchFilter{T}"/>
/// That terminates the recursive chain on a <see cref="SearchOperation{T}"/>
/// </summary>
public class SearchFilterWhere : SearchFilter
{
    public required SearchOperation Where { get; init; }

    /// <inheritdoc/>
    public override Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        return Where.Compile(ctx);
    }
}
