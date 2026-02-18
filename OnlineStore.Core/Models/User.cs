using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    
    // Поля для сброса пароля
    public string? ResetTokenHash { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    
    // Навигационные свойства
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<FavoriteItem> FavoriteItems { get; set; } = new List<FavoriteItem>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
}