using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using FluentValidation;
using OnlineStore.Core.Models;
using OnlineStore.Core;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Services;
using OnlineStore.Services.Services;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;

namespace OnlineStore.API.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации и авторизации пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<ForgotPasswordDTO> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordDTO> _resetPasswordValidator;
        private readonly IValidator<ChangePasswordDTO> _changePasswordValidator;
        private readonly IValidator<ConfirmEmailDTO> _confirmEmailValidator;
        private readonly IJwtService _jwtService;
        private readonly IAuditService _auditService;

        public AuthController(
            OnlineStoreDbContext context,
            IValidator<RegisterDTO> registerValidator,
            IValidator<ForgotPasswordDTO> forgotPasswordValidator,
            IValidator<ResetPasswordDTO> resetPasswordValidator,
            IValidator<ChangePasswordDTO> changePasswordValidator,
            IValidator<ConfirmEmailDTO> confirmEmailValidator,
            IJwtService jwtService,
            IAuditService auditService)
        {
            _context = context;
            _registerValidator = registerValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _jwtService = jwtService;
            _auditService = auditService;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="dto">Данные для регистрации</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат регистрации</returns>
        /// <response code="200">Пользователь успешно зарегистрирован</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            // Получение IP и UserAgent для логирования
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
            var email = dto.Email;
            
            // Валидация данных
            var validationResult = await _registerValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Логирование неудачной попытки регистрации
                await _auditService.LogEventAsync(
                    SecurityEventType.Register,
                    null,
                    email,
                    ipAddress,
                    userAgent,
                    false,
                    new { Errors = validationResult.Errors });
                    
                return BadRequest(validationResult.Errors);
            }

            // Проверка существующих email и username
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Username == dto.Username, cancellationToken);
                
            if (existingUser != null)
            {
                // Логирование неудачной попытки регистрации
                await _auditService.LogEventAsync(
                    SecurityEventType.Register,
                    null,
                    dto.Email,
                    ipAddress,
                    userAgent,
                    false,
                    new { Errors = "Email or username already exists" });
                    
                if (existingUser.Email == dto.Email)
                {
                    return BadRequest("Пользователь с таким email уже существует");
                }
                else
                {
                    return BadRequest("Пользователь с таким именем уже существует");
                }
            }

            // Хеширование пароля
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Создание пользователя
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false // По умолчанию email не подтвержден
            };

            // Сохранение пользователя
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Логирование успешной регистрации
            await _auditService.LogEventAsync(
                SecurityEventType.Register,
                user.Id,
                user.Email,
                ipAddress,
                userAgent,
                true,
                new { UserId = user.Id, Username = user.Username });

            // Назначение роли "User"
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь", cancellationToken);
            if (userRole != null)
            {
                var userRoleAssignment = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow
                };
                _context.UserRoles.Add(userRoleAssignment);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Генерация confirmation token (mock реализация)
            var confirmationToken = Guid.NewGuid().ToString();

            // Отправка welcome email (mock реализация)
            // В реальной реализации здесь будет вызов сервиса отправки email
            Console.WriteLine($"Отправка email подтверждения на {dto.Email} с токеном {confirmationToken}");

            return Ok(new { Message = "Пользователь успешно зарегистрирован", UserId = user.Id });
        }
        
        /// <summary>
        /// Вход пользователя в систему
        /// </summary>
        /// <param name="dto">Данные для входа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Токены доступа и обновления</returns>
        /// <response code="200">Успешный вход в систему</response>
        /// <response code="401">Неверные учетные данные</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO dto, CancellationToken cancellationToken = default)
        {
            // Поиск пользователя по email ИЛИ по username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.EmailOrUsername || u.Username == dto.EmailOrUsername, cancellationToken);
            
            // Проверка существования пользователя
            if (user == null)
            {
                // Логирование неудачной попытки входа
                await _auditService.LogEventAsync(
                    SecurityEventType.FailedLogin,
                    null,
                    dto.EmailOrUsername,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "User not found" });
                    
                return Unauthorized("Неверные учетные данные");
            }
            
            // Проверки: IsActive == true, IsDeleted == false
            if (!user.IsActive || user.IsDeleted)
            {
                // Логирование неудачной попытки входа
                await _auditService.LogEventAsync(
                    SecurityEventType.FailedLogin,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Account inactive or deleted" });
                    
                return Unauthorized("Аккаунт не активен или удален");
            }
            
            // Проверка, что email подтвержден (кроме администраторов)
            var isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Администратор");
            if (!user.IsEmailConfirmed && !isAdmin)
            {
                // Логирование неудачной попытки входа
                await _auditService.LogEventAsync(
                    SecurityEventType.FailedLogin,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Email not confirmed" });
                    
                return BadRequest("Email не подтвержден");
            }
            
            // Проверка пароля через BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                // Логирование неудачной попытки входа
                await _auditService.LogEventAsync(
                    SecurityEventType.FailedLogin,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Invalid password" });
                    
                return Unauthorized("Неверные учетные данные");
            }
            
            // Получение ролей пользователя
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            // Генерация Access Token с claims: UserId, Email, Username, FirstName, LastName, Roles
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            
            // Генерация Refresh Token
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            // Сохранение RefreshToken в UserSession
            var userSession = new UserSession
            {
                UserId = user.Id,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // TODO: Получить из настроек
                DeviceInfo = HttpContext.Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                IsRevoked = false
            };
            
            _context.UserSessions.Add(userSession);
            
            // Обновление LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            _context.Users.Update(user);
            
            await _context.SaveChangesAsync(cancellationToken);
            
            // Логирование успешного входа
            await _auditService.LogEventAsync(
                SecurityEventType.Login,
                user.Id,
                user.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers.UserAgent.ToString(),
                true,
                new { SessionId = userSession.Id });
            
            // Формирование ответа
            var response = new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(20), // TODO: Получить из настроек
                RefreshTokenExpiry = userSession.ExpiresAt,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    LastLoginAt = user.LastLoginAt,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = roles
                }
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Обновление токенов доступа
        /// </summary>
        /// <param name="dto">Данные для обновления токенов</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Новые токены доступа и обновления</returns>
        /// <response code="200">Токены успешно обновлены</response>
        /// <response code="401">Недействительный токен</response>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDTO>> Refresh([FromBody] RefreshTokenRequestDTO dto, CancellationToken cancellationToken = default)
        {
            // Извлечение UserId из истёкшего access token
            ClaimsPrincipal principal;
            try
            {
                principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken);
            }
            catch (SecurityTokenMalformedException ex)
            {
                // Логирование неудачной попытки обновления токена из-за неверного формата токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    null,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Malformed token", Message = ex.Message, Token = dto.AccessToken });
                    
                return Unauthorized("Недействительный токен");
            }
            catch (Exception ex)
            {
                // Логирование неудачной попытки обновления токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    null,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Token validation error", Message = ex.Message, Token = dto.AccessToken });
                    
                return Unauthorized("Недействительный токен");
            }
            
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null)
            {
                // Логирование неудачной попытки обновления токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    null,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Invalid token - missing user ID claim" });
                    
                return Unauthorized("Недействительный токен");
            }
            
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Недействительный токен");
            }
            
            // Поиск пользователя
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            
            if (user == null)
            {
                // Логирование неудачной попытки обновления токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    null,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "User not found" });
                    
                return Unauthorized("Пользователь не найден");
            }
            
            // Проверка RefreshToken в UserSession
            var userSessions = await _context.UserSessions
                .Where(us => us.UserId == userId && !us.IsRevoked)
                .ToListAsync(cancellationToken);
                
            UserSession? userSession = null;
            foreach (var session in userSessions)
            {
                if (BCrypt.Net.BCrypt.Verify(dto.RefreshToken, session.RefreshTokenHash))
                {
                    userSession = session;
                    break;
                }
            }
            
            if (userSession == null)
            {
                // Логирование неудачной попытки обновления токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Session not found" });
                    
                return Unauthorized("Сессия не найдена");
            }
            
            
            // Проверка, что RefreshToken не истек
            if (userSession.ExpiresAt < DateTime.UtcNow)
            {
                // Логирование неудачной попытки обновления токена
                await _auditService.LogEventAsync(
                    SecurityEventType.TokenRefresh,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Reason = "Refresh token expired" });
                    
                return Unauthorized("Refresh token истек");
            }
            
            // Получение ролей пользователя
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            // Генерация новой пары токенов
            var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            
            // Ротация RefreshToken
            userSession.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            userSession.ExpiresAt = DateTime.UtcNow.AddDays(7); // TODO: Получить из настроек
            
            // Сохранение нового RefreshToken в БД
            _context.UserSessions.Update(userSession);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Логирование успешного обновления токена
            await _auditService.LogEventAsync(
                SecurityEventType.TokenRefresh,
                user.Id,
                user.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers.UserAgent.ToString(),
                true,
                new { SessionId = userSession.Id });
            
            // Формирование ответа
            var response = new LoginResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(20), // TODO: Получить из настроек
                RefreshTokenExpiry = userSession.ExpiresAt,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    LastLoginAt = user.LastLoginAt,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = roles
                }
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат выхода из системы</returns>
        /// <response code="200">Успешный выход из системы</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            // Получение UserId из Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null)
            {
                return Unauthorized("Пользователь не авторизован");
            }
            
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Недействительный токен");
            }
            
            // Отзыв всех сессий пользователя
            var userSessions = await _context.UserSessions
                .Where(us => us.UserId == userId && !us.IsRevoked)
                .ToListAsync(cancellationToken);
            
            foreach (var session in userSessions)
            {
                session.IsRevoked = true;
            }
            
            _context.UserSessions.UpdateRange(userSessions);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Ok(new { message = "Logged out successfully" });
        }
        
        /// <summary>
        /// Отзыв всех сессий пользователя
        /// </summary>
        /// <param name="token">JWT токен (опционально, если не используется заголовок Authorization)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат отзыва сессий</returns>
        /// <response code="200">Все сессии отозваны</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("revoke")]
        [Authorize]
        public async Task<ActionResult> Revoke([FromQuery] string? token = null, CancellationToken cancellationToken = default)
        {
            // Получение UserId из Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
            // Если не удалось получить Claims из заголовка, пробуем из параметра token
            if (userIdClaim == null && !string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = _jwtService.GetPrincipalFromToken(token);
                    userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки валидации токена
                    await _auditService.LogEventAsync(
                        SecurityEventType.Logout,
                        null,
                        null,
                        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        HttpContext.Request.Headers.UserAgent.ToString(),
                        false,
                        new { Reason = "Token validation error", Error = ex.Message });
                    
                    return Unauthorized("Недействительный токен");
                }
            }
            
            if (userIdClaim == null)
            {
                return Unauthorized("Пользователь не авторизован");
            }
            
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Недействительный токен");
            }
            
            // Отзыв всех сессий пользователя (аналогично logout)
            var userSessions = await _context.UserSessions
                .Where(us => us.UserId == userId && !us.IsRevoked)
                .ToListAsync(cancellationToken);
            
            foreach (var session in userSessions)
            {
                session.IsRevoked = true;
            }
            
            _context.UserSessions.UpdateRange(userSessions);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Ok(new { Message = "Все сессии отозваны" });
        }
        
        /// <summary>
        /// Запрос на сброс пароля
        /// </summary>
        /// <param name="dto">Данные для сброса пароля</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат запроса на сброс пароля</returns>
        /// <response code="200">Запрос на сброс пароля успешно отправлен</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            // Валидация данных
            var validationResult = await _forgotPasswordValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            // Поиск пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive && !u.IsDeleted, cancellationToken);
            
            // Всегда возвращаем success, даже если email не найден
            if (user == null)
            {
                // Логирование попытки сброса пароля для несуществующего email
                await _auditService.LogEventAsync(
                    SecurityEventType.PasswordResetRequest,
                    null,
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    true,
                    new { Message = "Password reset requested for non-existent email" });
                    
                return Ok(new { Message = "Если email существует, на него будет отправлена ссылка для сброса пароля" });
            }
            
            // Проверка, что email подтвержден
            if (!user.IsEmailConfirmed)
            {
                // Логирование попытки сброса пароля для неподтвержденного email
                await _auditService.LogEventAsync(
                    SecurityEventType.PasswordResetRequest,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Message = "Password reset requested for unconfirmed email" });
                    
                return Ok(new { Message = "Если email существует, на него будет отправлена ссылка для сброса пароля" });
            }
            
            // Генерация криптографически стойкого reset token
            var resetToken = Guid.NewGuid().ToString("N");
            var resetTokenHash = BCrypt.Net.BCrypt.HashPassword(resetToken);
            
            // Сохранение token и expiry в БД
            user.ResetTokenHash = resetTokenHash;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Токен действует 1 час
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Отправка email со ссылкой для сброса (mock реализация)
            // В реальной реализации здесь будет вызов сервиса отправки email
            Console.WriteLine($"Отправка email для сброса пароля на {dto.Email} с токеном {resetToken}");
            
            return Ok(new { Message = "Если email существует, на него будет отправлена ссылка для сброса пароля" });
        }
        
        /// <summary>
        /// Сброс пароля пользователя
        /// </summary>
        /// <param name="dto">Данные для сброса пароля</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат сброса пароля</returns>
        /// <response code="200">Пароль успешно изменен</response>
        /// <response code="400">Недействительный или истекший токен</response>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            // Валидация данных
            var validationResult = await _resetPasswordValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            // Поиск пользователя по токену
            var users = await _context.Users
                .Where(u => u.ResetTokenHash != null)
                .ToListAsync(cancellationToken);
                
            User? user = null;
            // Нормализуем токен, убирая дефисы для совместимости
            var normalizedToken = dto.Token.Replace("-", "");
            foreach (var u in users)
            {
                if (u.ResetTokenHash != null && BCrypt.Net.BCrypt.Verify(normalizedToken, u.ResetTokenHash))
                {
                    user = u;
                    break;
                }
            }
            
            // Проверка существования пользователя и токена
            if (user == null || user.ResetTokenHash == null)
            {
                return BadRequest("Недействительный или истекший токен");
            }
            
            // Проверка срока действия токена
            if (user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest("Недействительный или истекший токен");
            }
            
            
            // Хеширование нового пароля
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            
            // Сохранение нового пароля в БД
            user.PasswordHash = newPasswordHash;
            user.ResetTokenHash = null;
            user.ResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Отзыв всех refresh токенов пользователя
            var userSessions = await _context.UserSessions
                .Where(us => us.UserId == user.Id && !us.IsRevoked)
                .ToListAsync(cancellationToken);
            
            foreach (var session in userSessions)
            {
                session.IsRevoked = true;
            }
            
            _context.UserSessions.UpdateRange(userSessions);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Генерация новых токенов для автоматического входа после сброса пароля
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);
            
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            // Создание новой сессии
            var userSession = new UserSession
            {
                UserId = user.Id,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // TODO: Получить из настроек
                DeviceInfo = HttpContext.Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                IsRevoked = false
            };
            
            _context.UserSessions.Add(userSession);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Формирование ответа с новыми токенами
            var response = new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(20), // TODO: Получить из настроек
                RefreshTokenExpiry = userSession.ExpiresAt,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    LastLoginAt = user.LastLoginAt,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = roles
                }
            };
            
            return Ok(new {
                Message = "Пароль успешно изменен",
                Tokens = response
            });
        }
        
        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="dto">Данные для изменения пароля</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат изменения пароля</returns>
        /// <response code="200">Пароль успешно изменен</response>
        /// <response code="400">Ошибка валидации данных или неверный текущий пароль</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO dto, CancellationToken cancellationToken = default)
        {
            // Валидация данных
            var validationResult = await _changePasswordValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            // Получение UserId из Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Пользователь не авторизован");
            }
            
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Недействительный токен");
            }
            
            // Поиск пользователя
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive && !u.IsDeleted, cancellationToken);
            if (user == null)
            {
                return Unauthorized("Пользователь не найден");
            }
            
            // Проверка текущего пароля
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                // Логирование неудачной попытки изменения пароля
                await _auditService.LogEventAsync(
                    SecurityEventType.PasswordChange,
                    user.Id,
                    user.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers.UserAgent.ToString(),
                    false,
                    new { Message = "Invalid current password" });
                    
                return BadRequest("Неверный текущий пароль");
            }
            
            // Хеширование и сохранение нового пароля
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Логирование успешного изменения пароля
            await _auditService.LogEventAsync(
                SecurityEventType.PasswordChange,
                user.Id,
                user.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers.UserAgent.ToString(),
                true,
                new { Message = "Password successfully changed" });
            
            return Ok(new { Message = "Пароль успешно изменен" });
        }
        
        /// <summary>
        /// Подтверждение email пользователя
        /// </summary>
        /// <param name="dto">Данные для подтверждения email</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат подтверждения email</returns>
        /// <response code="200">Email успешно подтвержден</response>
        /// <response code="400">Недействительный запрос или токен</response>
        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO dto, CancellationToken cancellationToken = default)
        {
            // Валидация данных
            var validationResult = await _confirmEmailValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            // Поиск пользователя по UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId && u.IsActive && !u.IsDeleted, cancellationToken);
            if (user == null)
            {
                return BadRequest("Недействительный запрос");
            }
            
            // Проверка, что email еще не подтвержден
            if (user.IsEmailConfirmed)
            {
                return Ok(new { Message = "Email уже подтвержден" });
            }
            
            // Проверка токена
            // В реальной реализации здесь будет проверка криптографического токена
            // Для упрощения примера используем простую проверку
            if (string.IsNullOrEmpty(dto.Token))
            {
                return BadRequest("Недействительный токен");
            }
            
            // Установка IsEmailConfirmed = true
            user.IsEmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Логирование успешного подтверждения email
            await _auditService.LogEventAsync(
                SecurityEventType.EmailConfirmation,
                user.Id,
                user.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers.UserAgent.ToString(),
                true,
                new { Message = "Email successfully confirmed" });
            
            return Ok(new { Message = "Email успешно подтвержден" });
        }
        
        /// <summary>
        /// Получение профиля пользователя
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Данные профиля пользователя</returns>
        /// <response code="200">Профиль пользователя успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserResponseDTO>> Profile(CancellationToken cancellationToken = default)
        {
            // Получение UserId из Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Пользователь не авторизован");
            }
            
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Недействительный токен");
            }
            
            // Загрузка пользователя с ролями
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive && !u.IsDeleted, cancellationToken);
            
            if (user == null)
            {
                return Unauthorized("Пользователь не найден");
            }
            
            // Формирование UserResponseDTO
            var userResponse = new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
            
            return Ok(userResponse);
        }
    }
}