using Microsoft.OpenApi.Models;
using Nomadik.Core;

namespace Nomadik.Extensions.Swagger;

/// <summary>
///
/// </summary>
public class SearchQuerySchema : OpenApiSchema
{
    public SearchQuerySchema(
        string searchFilterId, string searchOrderId
    )
    {
        Type = "object";

        Properties["filter"] = new()
        {
            Nullable = true,
            Reference = new()
            {
                Id = searchFilterId,
                Type = ReferenceType.Schema
            }
        };

        Properties["order"] = new()
        {
            Nullable = true,
            Reference = new()
            {
                Id = searchOrderId,
                Type = ReferenceType.Schema
            }
        };

        Properties["page"] = new()
        {
            Nullable = true,
            Reference = new()
            {
                Id = nameof(SearchPagination),
                Type = ReferenceType.Schema
            }
        };
    }
}
