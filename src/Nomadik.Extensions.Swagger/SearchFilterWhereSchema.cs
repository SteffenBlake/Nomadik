using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

public class SearchFilterWhereSchema : OpenApiSchema
{
    public SearchFilterWhereSchema(string searchOperationId)
    {
        Type = "object";
        Properties["where"] = new ()
        {
            Reference = new()
            {
                Id = searchOperationId,
                Type = ReferenceType.Schema
            }
        };
    }
}
