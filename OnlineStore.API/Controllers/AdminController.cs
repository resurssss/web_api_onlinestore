using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;

namespace OnlineStore.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Администратор")]
public class AdminController : ControllerBase
{
    private readonly IAuditService _auditService;
    
    public AdminController(IAuditService auditService)
    {
        _auditService = auditService;
    }
    
    /// <summary>
    /// Получение всех логов безопасности с пагинацией и фильтрами
    /// </summary>
    /// <param name="page">Номер страницы (по умолчанию 1)</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
    /// <param name="eventType">Тип события (опционально)</param>
    /// <param name="userId">ID пользователя (опционально)</param>
    /// <param name="startDate">Дата начала (опционально)</param>
    /// <param name="endDate">Дата окончания (опционально)</param>
    /// <returns>Список логов безопасности</returns>
    [HttpGet("audit")]
    public async Task<ActionResult<IEnumerable<SecurityAuditLog>>> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] SecurityEventType? eventType = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Ограничение на размер страницы
        
        var logs = await _auditService.GetAuditLogsAsync(page, pageSize, eventType, userId, startDate, endDate);
        var totalCount = await _auditService.GetAuditLogsCountAsync(eventType, userId, startDate, endDate);
        
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("X-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());
        Response.Headers.Append("X-Page-Count", ((int)Math.Ceiling((double)totalCount / pageSize)).ToString());
        
        return Ok(logs);
    }
    
    /// <summary>
    /// Получение логов безопасности для конкретного пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="page">Номер страницы (по умолчанию 1)</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
    /// <returns>Список логов безопасности пользователя</returns>
    [HttpGet("audit/user/{userId}")]
    public async Task<ActionResult<IEnumerable<SecurityAuditLog>>> GetUserAuditLogs(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Ограничение на размер страницы
        
        var logs = await _auditService.GetUserAuditLogsAsync(userId, page, pageSize);
        return Ok(logs);
    }
    
    /// <summary>
    /// Получение логов подозрительной активности
    /// </summary>
    /// <param name="page">Номер страницы (по умолчанию 1)</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
    /// <returns>Список логов подозрительной активности</returns>
    [HttpGet("audit/suspicious")]
    public async Task<ActionResult<IEnumerable<SecurityAuditLog>>> GetSuspiciousActivity(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Ограничение на размер страницы
        
        var logs = await _auditService.GetSuspiciousActivityAsync(page, pageSize);
        return Ok(logs);
    }
}