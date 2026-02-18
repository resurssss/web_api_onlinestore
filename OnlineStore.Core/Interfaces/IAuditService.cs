using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces;

public interface IAuditService
{
    Task LogEventAsync(SecurityEventType eventType, int? userId, string? email, string? ipAddress, 
        string? userAgent, bool success, object? details = null);
    
    Task<List<SecurityAuditLog>> GetAuditLogsAsync(int page, int pageSize, SecurityEventType? eventType = null, 
        int? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        
    Task<List<SecurityAuditLog>> GetUserAuditLogsAsync(int userId, int page, int pageSize);
    
    Task<List<SecurityAuditLog>> GetSuspiciousActivityAsync(int page, int pageSize);
    
    Task<int> GetAuditLogsCountAsync(SecurityEventType? eventType = null, int? userId = null, 
        DateTime? startDate = null, DateTime? endDate = null);
}