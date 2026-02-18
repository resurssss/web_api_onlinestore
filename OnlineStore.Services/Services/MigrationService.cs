using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineStore.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Services.Services
{
    public class MigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationService> _logger;

        public MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Начало применения миграций...");

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
                
                // Применение миграций только в Development окружении
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Миграции успешно применены.");
                }
                else
                {
                    _logger.LogInformation("Пропущено применение миграций в окружении: {Environment}", environment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при применении миграций");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}