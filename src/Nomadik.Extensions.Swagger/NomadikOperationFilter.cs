using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Nomadik.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nomadik.Extensions.Swagger;

public class NomadikOperationFilter : IOperationFilter
{
    /// <inheritdoc />
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
        var schemas = context.SchemaRepository.Schemas;

        var propertyEnumId = prefix + "PropertyEnum";
        if (!schemas.ContainsKey(propertyEnumId))
        {
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

            schemas[propertyEnumId] = new PropertyEnumSchema(props);
        }

        var searchOrderId = prefix + nameof(SearchOrder);
        if (!schemas.ContainsKey(searchOrderId))
        {
            var original = schemas[nameof(SearchOrder)];
            schemas[searchOrderId] = new SearchOrderSchema(
                nameof(OrderDir), 
                propertyEnumId,
                original
            );
        }

        var operatorId = nameof(Operator);
        _ = EnsureSchema(context, typeof(Operator));

        var searchOperationId = prefix + nameof(SearchOperation);
        if (!schemas.ContainsKey(searchOperationId))
        {
            schemas[searchOperationId] = new SearchOperationSchema(
                operatorId, propertyEnumId
            );
        }

        var searchFilterWhereId = prefix + nameof(SearchFilterWhere);
        if (!schemas.ContainsKey(searchFilterWhereId))
        {
            schemas[searchFilterWhereId] = new SearchFilterWhereSchema(
                searchOperationId
            );
        }

        var searchFilterId = prefix + nameof(SearchFilter);
        var searchFilterAndId = prefix + nameof(SearchFilterAnd);
        var searchFilterOrId = prefix + nameof(SearchFilterOr);

        if (!schemas.ContainsKey(searchFilterId))
        {
            schemas[searchFilterId] = new SearchFilterSchema(
                searchFilterAndId,
                searchFilterOrId,
                searchFilterWhereId
            );
        }

        if (!schemas.ContainsKey(searchFilterAndId))
        {
            schemas[searchFilterAndId] =
                new SearchFilterAndSchema(searchFilterId);
        }

        if (!schemas.ContainsKey(searchFilterOrId))
        {
            schemas[searchFilterOrId] = new SearchFilterOrSchema(
                searchFilterId
            );
        }

        var searchQueryId = prefix + nameof(SearchQuery);
        if (!schemas.ContainsKey(searchQueryId))
        {
            schemas[searchQueryId] = new SearchQuerySchema(
                searchFilterId, searchOrderId
            );
        }

        foreach(var content in operation.RequestBody.Content)
        {
            if (content.Value?.Schema?.Reference?.Id != nameof(SearchQuery))
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
            if (parameter.Schema?.Reference?.Id != nameof(SearchQuery))
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

    private static OpenApiSchema EnsureSchema(
        OperationFilterContext context,
        Type type
    )
    {
        if (context.SchemaRepository.TryLookupByType(type, out var schema))
        {
            return schema;
        }

        if (context.SchemaRepository.Schemas.TryGetValue(type.Name, out schema))
        {
            return schema;
        }

        return context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
    }
}
