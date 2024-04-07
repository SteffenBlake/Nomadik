using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class PropertyEnumSchema : OpenApiSchema
{
    public PropertyEnumSchema(List<string> properties)
    {
        Type = "string";
        Enum = properties.Select(o => 
            (IOpenApiAny)new OpenApiString(o)
        ).ToList();
    }
}
