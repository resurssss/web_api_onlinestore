using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using System.Text.Json;

namespace OnlineStore.Core.Services;

public class AuditService : IAuditService
{
    private readonly OnlineStoreDbContext _context;
    
    public AuditService(OnlineStoreDbContext context)
    {
        _context = context;
    }
    
    public async Task LogEventAsync(SecurityEventType eventType, int? userId, string? email, string? ipAddress, 
        string? userAgent, bool success, object? details = null)
    {
        var auditLog = new SecurityAuditLog
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            UserId = userId,
            Email = email,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Success = success,
            Details = details != null ? JsonSerializer.Serialize(details) : null,
            Timestamp = DateTime.UtcNow
        };
        
        _context.SecurityAuditLogs.Add(auditLog);
        
        // Асинхронное сохранение без блокировки основного потока
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Логирование ошибки сохранения (в реальной реализации использовать логгер)
            Console.WriteLine($"Ошибка сохранения лога аудита: {ex.Message}");
        }
    }
    
    public async Task<List<SecurityAuditLog>> GetAuditLogsAsync(int page, int pageSize, SecurityEventType? eventType = null, 
        int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SecurityAuditLogs.AsQueryable();
        
        if (eventType.HasValue)
            query = query.Where(x => x.EventType == eventType.Value);
            
        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);
            
        if (startDate.HasValue)
            query = query.Where(x => x.Timestamp >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(x => x.Timestamp <= endDate.Value);
            
        return await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<List<SecurityAuditLog>> GetUserAuditLogsAsync(int userId, int page, int pageSize)
    {
        return await _context.SecurityAuditLogs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<List<SecurityAuditLog>> GetSuspiciousActivityAsync(int page, int pageSize)
    {
        // Подозрительная активность: множественные неудачные попытки, 
        // попытка доступа к чужим ресурсам, использование истёкшего токена, 
        // необычный IP/User-Agent
        var suspiciousEventTypes = new[]
        {
            SecurityEventType.FailedLogin,
            SecurityEventType.SuspiciousActivity
        };
        
        return await _context.SecurityAuditLogs
            .Where(x => suspiciousEventTypes.Contains(x.EventType))
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<int> GetAuditLogsCountAsync(SecurityEventType? eventType = null, int? userId = null, 
        DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SecurityAuditLogs.AsQueryable();
        
        if (eventType.HasValue)
            query = query.Where(x => x.EventType == eventType.Value);
            
        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);
            
        if (startDate.HasValue)
            query = query.Where(x => x.Timestamp >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(x => x.Timestamp <= endDate.Value);
            
        return await query.CountAsync();
    }
}