using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace OnlineStore.Core;

public class OnlineStoreDbContextFactory : IDesignTimeDbContextFactory<OnlineStoreDbContext>
{
    public OnlineStoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OnlineStoreDbContext>();
        // Use PostgreSQL for design-time (migrations)
        // Connection string can be overridden via environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection")
                               ?? "Host=localhost;Port=5432;Database=onlinestore;Username=postgres;Password=postgres";
        optionsBuilder.UseNpgsql(connectionString)
            .ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

        return new OnlineStoreDbContext(optionsBuilder.Options);
    }
}