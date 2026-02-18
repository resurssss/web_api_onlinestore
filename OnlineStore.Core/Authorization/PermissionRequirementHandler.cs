using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Core.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly OnlineStoreDbContext _dbContext;

    public PermissionRequirementHandler(OnlineStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Получаем ID пользователя из клаймов
        var userIdClaim = context.User.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return;
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }

        // Получаем роли пользователя
        var userRoles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();

        // Проверяем, есть ли у пользователя необходимый permission
        var hasPermission = userRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Any(rp => rp.Permission.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}