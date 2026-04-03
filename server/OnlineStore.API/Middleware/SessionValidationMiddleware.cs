using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace OnlineStore.API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, OnlineStoreDbContext dbContext)
        {
            // Проверяем, есть ли заголовок Authorization
            var authHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Получаем UserId из токена
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // Проверяем, не отозвана ли сессия
                        var hasActiveSessions = await dbContext.UserSessions
                            .AnyAsync(s => s.UserId == userId && !s.IsRevoked);
                        
                        if (!hasActiveSessions)
                        {
                            // Сессия отозвана, возвращаем 401
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Session has been revoked");
                            return;
                        }
                    }
                }
            }
            
            // Продолжаем обработку запроса
            await _next(context);
        }
    }
}