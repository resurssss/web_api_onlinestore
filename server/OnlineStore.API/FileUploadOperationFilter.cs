using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        const string fileUploadMime = "multipart/form-data";
        var parameters = context.MethodInfo.GetParameters();
        
        // Проверяем, есть ли параметры с атрибутом [FromForm]
        var fromFormParameters = parameters
            .Where(p => p.GetCustomAttribute<FromFormAttribute>() != null)
            .ToList();
        
        // Также проверяем, есть ли параметры типа IFormFile
        var formFileParameters = parameters
            .Where(p => p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile) ||
                        (p.ParameterType.IsGenericType && 
                         p.ParameterType.GetGenericTypeDefinition() == typeof(List<>) &&
                         p.ParameterType.GetGenericArguments()[0] == typeof(Microsoft.AspNetCore.Http.IFormFile)))
            .ToList();
        
        var allFileParameters = fromFormParameters.Concat(formFileParameters).ToList();
        
        if (!allFileParameters.Any())
            return;

        // Настраиваем RequestBody для multipart/form-data
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [fileUploadMime] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>()
                    }
                }
            }
        };

        var mediaType = operation.RequestBody.Content[fileUploadMime];
        
        foreach (var parameter in allFileParameters)
        {
            // Удаляем параметр из обычных параметров операции
            var existingParameter = operation.Parameters
                .FirstOrDefault(p => p.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
            if (existingParameter != null)
            {
                operation.Parameters.Remove(existingParameter);
            }

            // Добавляем параметр в multipart/form-data schema
            var parameterType = parameter.ParameterType;
            var schema = CreateSchemaForType(parameterType);
            
            mediaType.Schema.Properties[parameter.Name] = schema;
        }
    }

    private OpenApiSchema CreateSchemaForType(Type type)
    {
        var schema = new OpenApiSchema();

        if (type == typeof(Microsoft.AspNetCore.Http.IFormFile))
        {
            schema.Type = "string";
            schema.Format = "binary";
        }
        else if (type == typeof(bool) || type == typeof(bool?))
        {
            schema.Type = "boolean";
        }
        else if (type == typeof(int) || type == typeof(int?))
        {
            schema.Type = "integer";
            schema.Format = "int32";
        }
        else if (type == typeof(long) || type == typeof(long?))
        {
            schema.Type = "integer";
            schema.Format = "int64";
        }
        else if (type == typeof(float) || type == typeof(float?) ||
                 type == typeof(double) || type == typeof(double?) ||
                 type == typeof(decimal) || type == typeof(decimal?))
        {
            schema.Type = "number";
        }
        else if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            schema.Type = "string";
            schema.Format = "date-time";
        }
        else if (type == typeof(string))
        {
            schema.Type = "string";
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            // Обработка List<IFormFile> и других списков
            var elementType = type.GetGenericArguments()[0];
            if (elementType == typeof(Microsoft.AspNetCore.Http.IFormFile))
            {
                schema.Type = "array";
                schema.Items = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
            else
            {
                schema.Type = "array";
                schema.Items = CreateSchemaForType(elementType);
            }
        }
        else
        {
            // Для всех остальных типов создаем простую схему
            schema.Type = "string";
        }

        return schema;
    }
}