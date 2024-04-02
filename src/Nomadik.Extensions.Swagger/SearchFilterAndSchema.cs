using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

public class SearchFilterAndSchema : OpenApiSchema
{
    public SearchFilterAndSchema(string searchFilterId)
    {
        Type = "object";
        Properties["and"] = new ()
        {
            Type = "array",
            Items = new()
            {
                Reference = new()
                {
                    Id = searchFilterId,
                    Type = ReferenceType.Schema
                }
            }
        };
    }
}
