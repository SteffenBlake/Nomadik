using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

/// <summary>
/// Core storage object for Mappings, pre-serializes a mapping into
/// its component expression parts. 
/// Preliminary class to serialize a Member Init <see cref=Expression/> 
/// into a <see cref="CompiledSearchQuery{TIn, TOut}"/> via
/// <see cref="Compile(SearchQuery)"/>
/// </summary>
/// <param name="mapping">
/// Preliminary Member Init Expression used to compile a <see cref="SearchQuery"/>
/// into a <see cref="CompiledSearchQuery{TIn, TOut}"/>
/// </param>
/// <param name="opHandlers">
/// Set of optional additional Operation Handlers to 
/// modify serialization behavior into the final Expression Tree
/// </param>
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
