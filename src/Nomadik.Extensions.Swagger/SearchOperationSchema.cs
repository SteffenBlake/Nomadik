using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class SearchOperationSchema : OpenApiSchema
{
    public SearchOperationSchema(
        string operatorId, string keyId
    )
    {
        Type = "object";
        Properties["key"] = new()
        {
            Nullable = false,
            Reference = new()
            {
                Id = keyId,
                Type = ReferenceType.Schema,
            }
        };
        Properties["operator"] = new()
        {
            Nullable = false,
            Reference = new()
            {
                Id = operatorId,
                Type = ReferenceType.Schema
            }
        };
        Properties["value"] = new OpenApiSchema();
    }
}
