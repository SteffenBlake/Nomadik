using Microsoft.Extensions.DependencyInjection;
using Nomadik.Core;
using Nomadik.Core.Abstractions;

namespace Nomadik.Extensions.DependencyInjection;

/// <summary>
/// Extends <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the full stack of Nomadik core services
    /// to the Dependency Injection engine.
    /// Inner options enable registering of individal Mappings,
    /// addition of new middleware, and modifying project wide configuration
    /// </summary>
    public static IServiceCollection AddNomadik(
        this IServiceCollection services,
        Action<NomadikOptions> options
    )
    {
        var config = new NomadikConfig();
        var nomadikOptions = new NomadikOptions(services, config);
        options(nomadikOptions);

        services.AddSingleton<INomadikConfig>(config);
        
        foreach (var opHandler in Nomadik.DefaultOpHandlers())
        {
            services.AddSingleton(opHandler);
        }

        return services;
    }

}
