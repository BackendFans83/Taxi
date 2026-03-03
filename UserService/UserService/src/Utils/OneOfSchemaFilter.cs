using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserService.Attributes;

namespace UserService.Utils;

public class OneOfSchemaFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requestAttribute = context.MethodInfo.GetCustomAttribute<SwaggerOneOfRequestAttribute>();
        if (requestAttribute != null && operation.RequestBody?.Content?.ContainsKey("application/json") == true)
        {
            var content = operation.RequestBody.Content["application/json"];
            CreateSchema(context, requestAttribute.Types, content);
        }

        var responseAttribute = context.MethodInfo.GetCustomAttribute<SwaggerOneOfResponseAttribute>();
        if (responseAttribute != null && operation.Responses!.ContainsKey("200"))
        {
            var response = operation.Responses["200"];
            if (response.Content?.ContainsKey("application/json") != true)
                return;
            var content = response.Content!["application/json"];
            CreateSchema(context, responseAttribute.Types, content);
        }
    }

    private static void CreateSchema(OperationFilterContext context, Type[] types, OpenApiMediaType mediaType)
    {
        var schemas = types
            .Select(type => context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository)).ToList();
        mediaType.Schema = new OpenApiSchema
        {
            OneOf = schemas,
            Description = $"Тип объекта: {string.Join(", ", types.Select(type => type.Name))}"
        };
    }
}