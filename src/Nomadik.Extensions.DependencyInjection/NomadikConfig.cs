using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Nomadik.Core;
using Nomadik.Core.Abstractions;

namespace Nomadik.Extensions.DependencyInjection;

public class NomadikOptions(
    IServiceCollection services,
    NomadikConfig config
)
{
    private readonly IServiceCollection _services = services;
    private readonly NomadikConfig _config = config;

    public IEqualityComparer<string> KeyComparer { get; set; }
        = StringComparer.InvariantCultureIgnoreCase;

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

    public void AddProviderSingleton<TProvider, TIn, TOut>()
        where TProvider : class, IMapperProvider<TIn, TOut>
    {
        _services.AddSingleton<IMapperProvider<TIn, TOut>, TProvider>();
        _services.AddSingleton<INomadik<TIn, TOut>, Nomadik<TIn, TOut>>();
    }

    public void AddProviderScoped<TProvider, TIn, TOut>()
        where TProvider : class, IMapperProvider<TIn, TOut>
    {
        _services.AddScoped<IMapperProvider<TIn, TOut>, TProvider>();
        _services.AddScoped<INomadik<TIn, TOut>, Nomadik<TIn, TOut>>();
    }

    public void AddOpHandler<THandler>()
        where THandler : class, INomadikOperationHandler
    {
        _services.AddSingleton<INomadikOperationHandler, THandler>();
    }

    public void Configure(Action<NomadikConfig> configure)
    {
        configure(_config);
    }
}
