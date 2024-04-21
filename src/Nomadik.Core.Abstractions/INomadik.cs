using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

/// <summary>
/// Core Nomadik Context for a given mapping, pre-caching its Mapping,
/// lookup table, and set of registered <see cref="INomadikOperationHandler"/>s
/// </summary>
public interface INomadik<TIn, TOut> 
{
    /// <summary>
    /// Expression Mapping used as baseline for all serialization
    /// </summary>
    Expression<Func<TIn, TOut>> Mapper { get; }

    /// <summary>
    /// Pre-calculated and cached table of Member expressions
    /// Built from <see cref="Mapper"/>
    /// </summary>
    IReadOnlyDictionary<string, Expression> Lookup { get; } 

    /// <summary>
    /// Set of registered Operation Handlers for compiling of
    /// "Where" Expression Trees
    /// </summary>
    IEnumerable<INomadikOperationHandler> OpHandlers { get; }
}
