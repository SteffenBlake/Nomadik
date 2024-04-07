using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public static class SwaggerGenOptionsExtensions 
{
    public static void AddNomadik(this SwaggerGenOptions options)
    {
        options.OperationFilter<NomadikOperationFilter>();
    }
}
