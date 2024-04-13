using System.Linq.Expressions;
using Nomadik.Core;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;
using Nomadik.Core.OperationHandlers;

namespace Nomadik;

/// <inheritdoc/>
public class Nomadik<TIn, TOut> : INomadik<TIn, TOut>
{
    public Nomadik(
        Expression<Func<TIn, TOut>> mapping,
        IReadOnlyDictionary<string, Expression> lookup,
        IEnumerable<INomadikOperationHandler> opHandlers
    )
    {
        Mapper = mapping;
        Lookup = lookup;
        OpHandlers = opHandlers;
    }

    public Nomadik(
        Expression<Func<TIn, TOut>> mapping,
        INomadikConfig config,
        IEnumerable<INomadikOperationHandler> opHandlers
    ) : this(mapping, mapping.ToMemberTable(config.KeyComparer), opHandlers)
    {}

    public Nomadik(
        IMapperProvider<TIn, TOut> provider,
        INomadikConfig config,
        IEnumerable<INomadikOperationHandler> opHandlers
    ) : this(provider.Compile(), config, opHandlers)
    {}

    /// <inheritdoc/>
    public Expression<Func<TIn, TOut>> Mapper { get; }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, Expression> Lookup { get; }

    /// <inheritdoc/>
    public IEnumerable<INomadikOperationHandler> OpHandlers { get; }
}

public static class Nomadik
{
    /// <summary>
    /// Compiles this <see cref="INomadik{TIn, TOut}"/> against a
    /// hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public static CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        SearchQuery query, 
        Expression<Func<TIn, TOut>> mapper
    )
    {
        return Compile(query, mapper, new NomadikConfig());
    }

    /// <summary>
    /// Compiles this <see cref="INomadik{TIn, TOut}"/> against a
    /// hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public static CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        SearchQuery query, 
        Expression<Func<TIn, TOut>> mapper,
        INomadikConfig config
    )
    {
        return Compile(query, mapper, config, DefaultOpHandlers());
    }

    /// <summary>
    /// Compiles this <see cref="INomadik{TIn, TOut}"/> against a
    /// hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public static CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        SearchQuery query, 
        Expression<Func<TIn, TOut>> mapper,
        IEnumerable<INomadikOperationHandler> opHandlers
    )
    {
        var context = new Nomadik<TIn, TOut>(
            mapper,
            new NomadikConfig(),
            opHandlers
        );
        return new(context, query);
    }

    /// <summary>
    /// Compiles this <see cref="INomadik{TIn, TOut}"/> against a
    /// hard typed InitMember Expression, producing a
    /// <see cref="CompiledSearchQuery{TIn, TOut}"/> that can be used
    /// to perform Filter, Sort, and Pagination operations on matching
    /// <see cref="IQueryable{T}"/> queries.
    /// </summary>
    public static CompiledSearchQuery<TIn, TOut> Compile<TIn, TOut>(
        SearchQuery query, 
        Expression<Func<TIn, TOut>> mapper,
        INomadikConfig config,
        IEnumerable<INomadikOperationHandler> opHandlers
    )
    {
        var context = new Nomadik<TIn, TOut>(
            mapper,
            config,
            opHandlers
        );
        return new(context, query);
    }

    public static IEnumerable<INomadikOperationHandler> DefaultOpHandlers()
    {
        yield return new ListOperationHandler();
        yield return new StringOperationHandler();
        yield return new DefaultOperationHandler();
    }
}
