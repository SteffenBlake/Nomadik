using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Nomadik.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nomadik.Extensions.Swagger;

public class NomadikOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach(var attribute in context.ApiDescription.CustomAttributes())
        {
            if (attribute is NomadikSearchAttribute nomadikSearch)
            {
                TryApplySearchAttribute(operation, context, nomadikSearch);
            }
        }
    }

    private static void TryApplySearchAttribute(
        OpenApiOperation operation, 
        OperationFilterContext context,
        NomadikSearchAttribute nomadikSearch
    )
    {
        var prefix = nomadikSearch.DTO.Name;

        var props = nomadikSearch.DTO.GetProperties()
            .Where(p =>
                p.GetMethod != null &&
                p.GetMethod.IsPublic &&
                !p.CustomAttributes.Any(a => 
                    a.AttributeType == typeof(NomadikIgnoreAttribute)
                )
            )
            .Select(a => a.Name)
            .ToList();

        var propertyEnumId = prefix + "PropertyEnum";
        if (!context.SchemaRepository.Schemas.ContainsKey(propertyEnumId))
        {
            context.SchemaRepository.Schemas[propertyEnumId] =
                new PropertyEnumSchema(props);
        }

        var searchOrderId = prefix + nameof(SearchOrder);
        if (!context.SchemaRepository.Schemas.ContainsKey(searchOrderId))
        {
            context.SchemaRepository.Schemas[searchOrderId] =
                new SearchOrderSchema(nameof(OrderDir), propertyEnumId);
        }

        var operatorId = nameof(Operator);
        if (!context.SchemaRepository.Schemas.ContainsKey(operatorId))
        {
            context.SchemaRepository.Schemas[operatorId] =
                new OpenApiSchema()
                {
                    Type = "string",
                    Enum = Enum.GetNames<Operator>().Select(o =>
                        (IOpenApiAny)new OpenApiString(o)
                    ).ToList()
                };
        }

        var searchOperationId = prefix + nameof(SearchOperation);
        if (!context.SchemaRepository.Schemas.ContainsKey(searchOperationId))
        {
            context.SchemaRepository.Schemas[searchOperationId] =
                new SearchOperationSchema(operatorId, propertyEnumId);
        }

        var searchFilterWhereId = prefix + nameof(SearchFilterWhere);
        if (!context.SchemaRepository.Schemas.ContainsKey(searchFilterWhereId))
        {
            context.SchemaRepository.Schemas[searchFilterWhereId] =
                new SearchFilterWhereSchema(searchOperationId);
        }

        var searchFilterId = prefix + nameof(SearchFilter);
        var searchFilterAndId = prefix + nameof(SearchFilterAnd);
        var searchFilterOrId = prefix + nameof(SearchFilterOr);

        if (!context.SchemaRepository.Schemas.ContainsKey(searchFilterId))
        {
            context.SchemaRepository.Schemas[searchFilterId] =
                new SearchFilterSchema(
                    searchFilterAndId,
                    searchFilterOrId,
                    searchFilterWhereId
                );
        }

        if (!context.SchemaRepository.Schemas.ContainsKey(searchFilterAndId))
        {
            context.SchemaRepository.Schemas[searchFilterAndId] =
                new SearchFilterAndSchema(searchFilterId);
        }

        if (!context.SchemaRepository.Schemas.ContainsKey(searchFilterOrId))
        {
            context.SchemaRepository.Schemas[searchFilterOrId] =
                new SearchFilterOrSchema(searchFilterId);
        }

        var searchQueryId = prefix + nameof(SearchQuery);
        if (!context.SchemaRepository.Schemas.ContainsKey(searchQueryId))
        {
            context.SchemaRepository.Schemas[searchQueryId] =
                new SearchQuerySchema(searchFilterId, searchOrderId);
        }

        foreach(var content in operation.RequestBody.Content)
        {
            if (content.Value.Schema.Reference.Id != nameof(SearchQuery))
            {
                continue;
            }

            content.Value.Schema.Reference = new ()
            {
                Id = searchQueryId,
                Type = ReferenceType.Schema
            };
        }

        foreach(var parameter in operation.Parameters)
        {
            if (parameter.Schema.Reference.Id != nameof(SearchQuery))
            {
                continue;
            }
            parameter.Schema.Reference = new ()
            {
                Id = searchQueryId,
                Type = ReferenceType.Schema
            };
        }
    }
}
