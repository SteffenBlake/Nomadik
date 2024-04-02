using Microsoft.OpenApi.Models;
using Nomadik.Core;

namespace Nomadik.Extensions.Swagger;

public class SearchQuerySchema : OpenApiSchema
{
    public SearchQuerySchema(string searchFilterId, string searchOrderId)
    {
        Type = "object";

        Properties["filter"] = new()
        {
            Reference = new()
            {
                Id = searchFilterId,
                Type = ReferenceType.Schema
            }
        };

        Properties["order"] = new()
        {
            Reference = new()
            {
                Id = searchOrderId,
                Type = ReferenceType.Schema
            }
        };

        Properties["page"] = new()
        {
            Reference = new()
            {
                Id = nameof(SearchPagination),
                Type = ReferenceType.Schema
            }
        };
    }
}
