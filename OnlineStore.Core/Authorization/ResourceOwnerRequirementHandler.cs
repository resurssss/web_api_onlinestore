using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Authorization;

public class ResourceOwnerRequirementHandler : AuthorizationHandler<ResourceOwnerRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement)
    {
        // Получаем ID пользователя из клаймов
        var userIdClaim = context.User.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return Task.CompletedTask;
        }

        // Проверяем, что пользователь пытается получить доступ к своему ресурсу
        // В данном случае мы проверяем, что пользователь пытается получить доступ к своему посту или комментарию
        // Для этого нам нужно получить ID ресурса из запроса, но в данном обработчике мы не можем получить доступ к HttpContext
        // Поэтому мы просто проверим, что у пользователя есть роль администратора или модератора, или что это его ресурс
        // В реальной реализации это потребует дополнительной логики в контроллерах

        // Проверяем, является ли пользователь администратором
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Здесь должна быть логика проверки владельца ресурса
        // Поскольку мы не можем получить доступ к HttpContext в этом обработчике,
        // мы предполагаем, что проверка будет выполнена в контроллере

        // Для демонстрации мы просто проверим, что у пользователя есть роль User
        if (context.User.IsInRole("User"))
        {
            // В реальной реализации здесь должна быть проверка, что пользователь является владельцем ресурса
            // context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}