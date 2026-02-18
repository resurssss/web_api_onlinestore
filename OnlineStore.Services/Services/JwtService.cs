using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using OnlineStore.Core.Models;
using OnlineStore.Core;
using OnlineStore.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace OnlineStore.Services.Services;

// JwtSettings остаётся без изменений
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
    Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(string token, bool validateLifetime = true);
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly OnlineStoreDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        OnlineStoreDbContext context,
        IAuditService auditService,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _auditService = auditService;
        _logger = logger;
        
        ValidateJwtSettings();
    }

    private void ValidateJwtSettings()
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.Secret))
            throw new ArgumentException("JWT Secret cannot be null or empty");
            
        if (_jwtSettings.Secret.Length < 32)
            _logger.LogWarning("JWT Secret is shorter than recommended 32 characters. Consider using a longer key.");
            
        if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            throw new ArgumentException("JWT Issuer cannot be null or empty");
            
        if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            throw new ArgumentException("JWT Audience cannot be null or empty");
            
        if (_jwtSettings.AccessTokenExpirationMinutes <= 0)
            throw new ArgumentException("AccessTokenExpirationMinutes must be greater than 0");
    }

    public string GenerateAccessToken(User user, IEnumerable<string> roles)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (roles == null) throw new ArgumentNullException(nameof(roles));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        return GetPrincipalFromToken(token, false);
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        return GetPrincipalFromToken(token, true);
    }

    public async Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(string token, bool validateLifetime = true)
    {
        try
        {
            return GetPrincipalFromToken(token, validateLifetime);
        }
        catch (Exception ex)
        {
            await LogSecurityEventAsync(
                SecurityEventType.SuspiciousActivity,
                "Invalid token attempt",
                new { TokenPreview = token.Length > 50 ? token[..50] + "..." : token, Exception = ex.Message });
            
            throw;
        }
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        // Базовая проверка формата JWT
        if (!IsValidJwtFormat(token))
        {
            // Используем Task.Run для избежания deadlock
            Task.Run(async () => await LogSecurityEventAsync(
                SecurityEventType.SuspiciousActivity,
                "Invalid JWT format",
                new { TokenPreview = token.Length > 50 ? token[..50] + "..." : token })).Wait();
                
            throw new SecurityTokenMalformedException("Invalid JWT format");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = validateLifetime,
            ValidateIssuer = validateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = validateLifetime,
            ClockSkew = validateLifetime ? TimeSpan.FromMinutes(5) : TimeSpan.Zero,
            RequireSignedTokens = true,
            RequireExpirationTime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
        catch (SecurityTokenExpiredException ex)
        {
            if (!validateLifetime)
            {
                // Для expired tokens игнорируем эту ошибку
                tokenValidationParameters.ValidateLifetime = false;
                return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            }
            
            _logger.LogWarning(ex, "Token expired");
            throw;
        }
    }

    private bool IsValidJwtFormat(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.');
        // JWS имеет 3 части, JWE имеет 5 частей
        return parts.Length == 3 || parts.Length == 5;
    }

    private async Task LogSecurityEventAsync(
        SecurityEventType eventType,
        string description,
        object? additionalData = null) // Изменено: добавлен знак вопроса для nullable
    {
        try
        {
            await _auditService.LogEventAsync(
                eventType,
                null,
                null,
                "JwtService",
                "TokenValidation",
                false,
                new { Description = description, AdditionalData = additionalData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log security event");
        }
    }
}