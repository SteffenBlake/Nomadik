using System.Linq.Expressions;
using Nomadik.Core;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;
using Nomadik.Core.OperationHandlers;

namespace Nomadik;

/// <summary>
/// Concrete implementation of <see cref="INomadik{TIn, TOut}"/>.
///
/// Largely used as an injectable container, for most use cases consumers
/// should either interface with the static <see cref="Nomadik"/> API, or,
/// leverage 
/// <see href="/api/Nomadik.Extensions.DependencyInjection.html">
/// Nomadik.Extensions.DependencyInjection
/// </see>
/// and inject the abstract <see cref="INomadik{TIn, TOut}"/> instead.
/// </summary>
public class Nomadik<TIn, TOut>(
    Expression<Func<TIn, TOut>> mapping,
    IReadOnlyDictionary<string, Expression> lookup,
    IEnumerable<INomadikOperationHandler> opHandlers
) : INomadik<TIn, TOut>
{
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
    public Expression<Func<TIn, TOut>> Mapper { get; } = mapping;

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, Expression> Lookup { get; } = lookup;

    /// <inheritdoc/>
    public IEnumerable<INomadikOperationHandler> OpHandlers { get; } = opHandlers;
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
    /// <include 
    ///     file="XmlDocs/Nomadik/Compile.xml"
    ///     path="doc/member[@id='M:Nomadik.Nomadik.Compile``2(Nomadik.Core.SearchQuery,System.Linq.Expressions.Expression{System.Func{``0,``1}})']/*"
    /// />
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
    /// <include 
    ///     file="XmlDocs/Nomadik/Compile.xml"
    ///     path="doc/member[@id='M:Nomadik.Nomadik.Compile``2(Nomadik.Core.SearchQuery,System.Linq.Expressions.Expression{System.Func{``0,``1}},Nomadik.Core.Abstractions.INomadikConfig)']/*"
    /// />
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
    /// <include 
    ///     file="XmlDocs/Nomadik/Compile.xml"
    ///     path="doc/member[@id='M:Nomadik.Nomadik.Compile``2(Nomadik.Core.SearchQuery,System.Linq.Expressions.Expression{System.Func{``0,``1}},System.Collections.Generic.IEnumerable{Nomadik.Core.Abstractions.INomadikOperationHandler})']/*"
    /// />
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
    /// <include 
    ///     file="XmlDocs/Nomadik/Compile.xml"
    ///     path="doc/member[@id='M:Nomadik.Nomadik.Compile``2(Nomadik.Core.SearchQuery,System.Linq.Expressions.Expression{System.Func{``0,``1}},Nomadik.Core.Abstractions.INomadikConfig,System.Collections.Generic.IEnumerable{Nomadik.Core.Abstractions.INomadikOperationHandler})']/*"
    /// />
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

    /// <summary>
    /// The set of baseline Operation Handlers that Nomadik ships with.
    /// These will be the ones used by Nomadik if no further configuration
    /// is specified, handled in the following order:
    /// 1. <see cref="ListOperationHandler"/>
    /// 2. <see cref="StringOperationHandler"/>
    /// 3. <see cref="DefaultOperationHandler"/>
    /// </summary>
    public static IEnumerable<INomadikOperationHandler> DefaultOpHandlers()
    {
        yield return new ListOperationHandler();
        yield return new StringOperationHandler();
        yield return new DefaultOperationHandler();
    }
}
