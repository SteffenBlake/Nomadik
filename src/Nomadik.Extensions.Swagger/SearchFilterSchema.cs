using Microsoft.OpenApi.Models;

namespace Nomadik.Extensions.Swagger;

public class SearchFilterSchema : OpenApiSchema
{
    public SearchFilterSchema(
        string searchFilterAndId, 
        string searchFilterOrId, 
        string searchFilterWhereId
    )
    {
        OneOf =
        [
            new()
            {
                Reference = new()
                {
                    Id = searchFilterAndId,
                    Type = ReferenceType.Schema
                }
            },
            new()
            {
                Reference = new()
                {
                    Id = searchFilterOrId,
                    Type = ReferenceType.Schema
                }
            },
            new()
            {
                Reference = new()
                {
                    Id = searchFilterWhereId,
                    Type = ReferenceType.Schema
                }
            }
        ];
    }
}
