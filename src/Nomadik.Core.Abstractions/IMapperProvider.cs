using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

public interface IMapperProvider<TIn, TOut>
{
    public Expression<Func<TIn, TOut>> Compile();
}
