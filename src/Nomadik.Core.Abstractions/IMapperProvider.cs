using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

/// <summary>
/// Lazy loaded option for generating dynamic Expression Mappings
/// during Runtime
/// </summary>
public interface IMapperProvider<TIn, TOut>
{
    /// <summary>
    /// Lazy loads the Expression Mapping, called during runtime at last
    /// possible minute. Is not cached, and is called each time it is
    /// used seperately
    /// </summary>
    public Expression<Func<TIn, TOut>> Compile();
}
