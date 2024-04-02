using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nomadik.Extensions.Swagger;

public static class SwaggerGenOptionsExtensions 
{
    public static void AddNomadik(this SwaggerGenOptions options)
    {
        options.OperationFilter<NomadikOperationFilter>();
    }
}
