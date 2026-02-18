using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Authorization;

public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        // Проверяем, что у пользователя есть дата рождения
        var dateOfBirthClaim = context.User.FindFirst("DateOfBirth");
        if (dateOfBirthClaim == null)
        {
            return Task.CompletedTask;
        }

        // Проверяем, что дата рождения в правильном формате
        if (!DateTime.TryParse(dateOfBirthClaim.Value, out var dateOfBirth))
        {
            return Task.CompletedTask;
        }

        // Вычисляем возраст
        var age = DateTime.Now.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Now.AddYears(-age))
        {
            age--;
        }

        // Если возраст соответствует требованию, авторизация успешна
        if (age >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}