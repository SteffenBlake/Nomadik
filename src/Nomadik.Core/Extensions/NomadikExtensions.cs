using Nomadik.Core.Abstractions;

namespace Nomadik.Core.Extensions;

/// <summary>
/// Extends <see cref="INomadik{TIn, TOut}"
/// </summary>
public static class NomadikExtensions 
{
    /// <summary>
    /// Compiles this <see cref="INomadik{TIn, TOut}"/> against a
    /// hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public static CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        this INomadik<TIn, TOut> context,
        SearchQuery query
    )
    {
        return new(context, query);
    }
}
