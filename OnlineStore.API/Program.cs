using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineStore.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineStore.API.Extensions;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;
using Microsoft.OpenApi.Models;
using OnlineStore.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Core.Authorization;
using System.IdentityModel.Tokens.Jwt;
using OnlineStore.Core.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// ========== КРИТИЧНО: НАСТРОЙКИ ДЛЯ БОЛЬШИХ ФАЙЛОВ ==========
// Увеличиваем лимиты Kestrel для скачивания больших файлов
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 5L * 1024 * 1024 * 1024; // 5 ГБ
    serverOptions.Limits.MinRequestBodyDataRate = null; // Убираем ограничение минимальной скорости
    serverOptions.Limits.MinResponseDataRate = null; // Убираем ограничение на скорость отправки
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15); // Увеличиваем таймаут
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

// Настройки для IIS (если используется)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 5L * 1024 * 1024 * 1024; // 5 ГБ
});

// Увеличиваем лимиты для загрузки больших файлов (для multipart/form-data)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5L * 1024 * 1024 * 1024; // 5 ГБ
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.BufferBodyLengthLimit = long.MaxValue;
});
// ========== КОНЕЦ НАСТРОЕК ДЛЯ БОЛЬШИХ ФАЙЛОВ ==========

// Добавляем настройки FileStorage
builder.Services.Configure<OnlineStore.Core.Services.FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

// Добавляем привязку JwtSettings из конфигурации
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Регистрация DbContext
builder.Services.AddDbContext<OnlineStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Автоматическое применение миграций при запуске (только для Development)
builder.Services.AddHostedService<MigrationService>();

// ВАЖНО: Сначала добавляем Authentication и Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret is missing"))),
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<OnlineStoreDbContext>();
                var jwtToken = context.SecurityToken as JwtSecurityToken;
                var tokenId = jwtToken?.Id; // JTI claim
                
                // Проверяем, не отозвана ли сессия
                var hasActiveSessions = await dbContext.UserSessions
                    .AnyAsync(s => s.UserId == userId && !s.IsRevoked);
                
                if (!hasActiveSessions)
                {
                    context.Fail("Session has been revoked");
                }
            }
        },
        OnMessageReceived = context =>
        {
            // Проверяем, не отозван ли токен при каждом запросе
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Получаем UserId из токена
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(accessToken) is JwtSecurityToken jwtToken)
                {
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // Проверяем, не отозвана ли сессия
                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<OnlineStoreDbContext>();
                        var hasActiveSessions = dbContext.UserSessions
                            .Any(s => s.UserId == userId && !s.IsRevoked);
                        
                        if (!hasActiveSessions)
                        {
                            // Сессия отозвана, отклоняем запрос
                            context.Fail("Session has been revoked");
                        }
                    }
                }
            }
            
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Authentication failed");
            return Task.CompletedTask;
        }
    };
});

// Затем добавляем Authorization
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireModeratorOrAdmin", policy => policy.RequireRole("Admin", "Moderator"));
    
    // Claim-based policies
    options.AddPolicy("RequireEmailConfirmed", policy => policy.RequireClaim("EmailConfirmed", "True"));
    options.AddPolicy("RequirePremiumSubscription", policy => policy.RequireClaim("SubscriptionLevel", "Premium", "Enterprise"));
    
    // Custom policies
    options.AddPolicy("RequireMinimumAge", policy => policy.Requirements.Add(new OnlineStore.Core.Authorization.MinimumAgeRequirement(18)));
    options.AddPolicy("CanEditPost", policy => policy.Requirements.Add(new OnlineStore.Core.Authorization.PermissionRequirement("EditPost")));
    options.AddPolicy("CanDeleteUser", policy => policy.Requirements.Add(new OnlineStore.Core.Authorization.PermissionRequirement("DeleteUser")));
    options.AddPolicy("CanViewReports", policy => policy.Requirements.Add(new OnlineStore.Core.Authorization.PermissionRequirement("ViewReports")));
});

// Register authorization handlers
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, OnlineStore.Core.Authorization.MinimumAgeRequirementHandler>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, OnlineStore.Core.Authorization.PermissionRequirementHandler>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, OnlineStore.Core.Authorization.ResourceOwnerRequirementHandler>();

// Регистрация зависимостей через extension-методы
builder.Services
    .AddRepositories()    // Singleton репозитории
    .AddServices()        // Scoped сервисы
    .AddValidators()      // Transient валидаторы
    .AddJwtService();     // JWT сервис

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Настройка Swagger с JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineStore API", Version = "v1" });
    
    // Настройка Bearer token authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    c.OperationFilter<FileUploadOperationFilter>();
    
    // Включаем комментарии XML если есть
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Явно описываем типы ответов для контроллера Files
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.RelativePath?.StartsWith("api/Files/") == true)
        {
            return true;
        }
        return apiDesc.GroupName == null || apiDesc.GroupName == docName;
    });
});

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:3000", "https://localhost:3001", "https://localhost:5001", "http://localhost:5000")
                  .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
                  .WithHeaders("Authorization", "Content-Type", "Accept", "Range")
                  .AllowCredentials()
                  .WithExposedHeaders("Content-Disposition", "Accept-Ranges", "Content-Range", "Content-Length"); // Разрешаем доступ к заголовкам для работы с файлами
        });
});

var app = builder.Build();

// Настройка middleware в правильном порядке
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// CORS middleware должен идти до UseRouting
app.UseCors("AllowSpecificOrigins");

app.UseRouting();

// Аутентификация и авторизация должны идти в таком порядке
app.UseAuthentication();
// Добавляем middleware для проверки состояния сессии
//app.UseMiddleware<OnlineStore.API.Middleware.SessionValidationMiddleware>();
app.UseAuthorization();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStore API v1");
    c.RoutePrefix = "swagger"; // Доступ по /swagger
});

app.MapControllers();

app.Run();
