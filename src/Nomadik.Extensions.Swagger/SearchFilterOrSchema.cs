using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class SearchFilterOrSchema : OpenApiSchema
{
    public SearchFilterOrSchema(string searchFilterId)
    {
        Type = "object";
        Properties["or"] = new ()
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
