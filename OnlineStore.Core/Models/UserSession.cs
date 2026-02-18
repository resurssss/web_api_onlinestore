using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Core.Models;

public class UserSession : BaseEntity
    {
        public new Guid Id { get; set; }
        
        public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string RefreshTokenHash { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public string DeviceInfo { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    
    public bool IsRevoked { get; set; }
}