using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Nomadik.Core;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;

namespace Nomadik.Extensions.DependencyInjection;

/// <summary>
/// Fluent interface for configuring the injected settings for Nomadik
/// </summary>
public class NomadikOptions(
    IServiceCollection services,
    NomadikConfig config
)
{
    private readonly IServiceCollection _services = services;
    private readonly NomadikConfig _config = config;

    /// <summary>
    /// Registers a greedy loaded static Expression Mapper
    /// Mappings will be registered immediately as a singleton
    /// </summary>
    public void AddMapper<TIn, TOut>(Expression<Func<TIn, TOut>> mapper)
    {
        _services.AddSingleton<INomadik<TIn, TOut>>(s => 
            new Nomadik<TIn, TOut>(
                mapper,
                s.GetRequiredService<INomadikConfig>(),
                s.GetRequiredService<IEnumerable<INomadikOperationHandler>>()
            )
        );
    }

    /// <summary>
    /// Registers a Mapping Provider as a Singleton that will 
    /// be invoked lazily whenever 
    /// <see cref="NomadikExtensions.Compile{TIn, TOut}(INomadik{TIn, TOut}, SearchQuery)"/>
    /// Is invoked.
    /// </summary>
    public void AddProviderSingleton<TProvider, TIn, TOut>()
        where TProvider : class, IMapperProvider<TIn, TOut>
    {
        _services.AddSingleton<IMapperProvider<TIn, TOut>, TProvider>();
        _services.AddSingleton<INomadik<TIn, TOut>, Nomadik<TIn, TOut>>();
    }

    /// <summary>
    /// Registers a Mapping Provider as a Scoped Service that will 
    /// be invoked lazily whenever 
    /// <see cref="NomadikExtensions.Compile{TIn, TOut}(INomadik{TIn, TOut}, SearchQuery)"/>
    /// Is invoked.
    /// </summary>
    public void AddProviderScoped<TProvider, TIn, TOut>()
        where TProvider : class, IMapperProvider<TIn, TOut>
    {
        _services.AddScoped<IMapperProvider<TIn, TOut>, TProvider>();
        _services.AddScoped<INomadik<TIn, TOut>, Nomadik<TIn, TOut>>();
    }

    /// <summary>
    /// Adds a new Operation Handler Middleware, adding it to the start
    /// of the Operation Handler pipeline 
    /// (It will be called before any of Nomadik builtin Handlers)
    /// </summary>
    public void AddOpHandler<THandler>()
        where THandler : class, INomadikOperationHandler
    {
        _services.AddSingleton<INomadikOperationHandler, THandler>();
    }

    /// <summary>
    /// Mutate the project wide Nomadik configuration
    /// </summary>
    public void Configure(Action<NomadikConfig> configure)
    {
        configure(_config);
    }
}
