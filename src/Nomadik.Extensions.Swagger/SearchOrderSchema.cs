using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class SearchOrderSchema : OpenApiSchema
{
    public SearchOrderSchema(
        string dirId,
        string byId,
        OpenApiSchema original
    )
    {
        Type = "object";
        Description = original.Description;
        Properties["dir"] = new() 
        {
            Description = original.Properties["dir"].Description,
            Reference = new()
            {
                Id = dirId,
                Type = ReferenceType.Schema
            }
        };
        Properties["by"] = new ()
        {
            Description = original.Properties["by"].Description,
            Reference = new()
            {
                Id = byId,
                Type = ReferenceType.Schema
            }
        };
    }
}
