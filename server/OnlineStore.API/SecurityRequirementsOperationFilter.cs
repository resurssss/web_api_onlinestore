using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Проверяем атрибуты контроллера
        var controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [];
        var methodAttributes = context.MethodInfo.GetCustomAttributes(true);
        
        // Объединяем все атрибуты
        var allAttributes = controllerAttributes.Union(methodAttributes);
        
        // Проверяем наличие атрибутов авторизации
        var hasAuthorize = allAttributes.OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous = allAttributes.OfType<AllowAnonymousAttribute>().Any();
        
        // Если есть [Authorize] И НЕТ [AllowAnonymous] → добавляем замок
        if (hasAuthorize && !hasAllowAnonymous)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
        else
        {
            // Если нет [Authorize] ИЛИ есть [AllowAnonymous] → убираем замок
            operation.Security = new List<OpenApiSecurityRequirement>();
        }
    }
}