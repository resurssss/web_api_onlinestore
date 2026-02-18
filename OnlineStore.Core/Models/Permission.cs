namespace OnlineStore.Core.Models;

public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    // Навигационные свойства
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}