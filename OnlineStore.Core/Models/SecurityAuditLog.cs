using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace OnlineStore.Core.Models;

public class SecurityAuditLog
{
    [Key]
    public Guid Id { get; set; }
    
    public SecurityEventType EventType { get; set; }
    
    public int? UserId { get; set; }
    
    public string? Email { get; set; }
    
    [MaxLength(45)] // IPv4 or IPv6
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public bool Success { get; set; }
    
    public string? Details { get; set; } // JSON данные
    
    public DateTime Timestamp { get; set; }
    
    // Навигационное свойство
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}