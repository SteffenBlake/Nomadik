using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

public class SearchOperationSchema : OpenApiSchema
{
    public SearchOperationSchema(
        string operatorId, string keyId
    )
    {
        Type = "object";
        Properties["key"] = new()
        {
            Reference = new()
            {
                Id = keyId,
                Type = ReferenceType.Schema
            }
        };
        Properties["operator"] = new()
        {
            Reference = new()
            {
                Id = operatorId,
                Type = ReferenceType.Schema
            }
        };
        Properties["value"] = new OpenApiSchema();
    }
}
