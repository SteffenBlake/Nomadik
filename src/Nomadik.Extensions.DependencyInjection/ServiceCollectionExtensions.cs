using Microsoft.Extensions.DependencyInjection;
using Nomadik.Core;
using Nomadik.Core.Abstractions;

namespace Nomadik.Extensions.DependencyInjection;

/// <summary>
/// Extends <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
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
