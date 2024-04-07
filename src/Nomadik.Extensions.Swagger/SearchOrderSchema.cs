using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class SearchOrderSchema : OpenApiSchema
{
    public SearchOrderSchema(
        string dirId,
        string byId
    )
    {
        Type = "object";
        Properties["dir"] = new() 
        {
            Reference = new()
            {
                Id = dirId,
                Type = ReferenceType.Schema
            }
        };
        Properties["by"] = new ()
        {
            Reference = new()
            {
                Id = byId,
                Type = ReferenceType.Schema
            }
        };
    }
}
