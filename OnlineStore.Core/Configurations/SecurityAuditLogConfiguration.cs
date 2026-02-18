using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Configurations;

public class SecurityAuditLogConfiguration : IEntityTypeConfiguration<SecurityAuditLog>
{
    public void Configure(EntityTypeBuilder<SecurityAuditLog> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.EventType)
            .IsRequired();
            
        builder.Property(x => x.UserId)
            .IsRequired(false);
            
        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired(false);
            
        builder.Property(x => x.IpAddress)
            .HasMaxLength(45)
            .IsRequired(false);
            
        builder.Property(x => x.UserAgent)
            .HasMaxLength(512)
            .IsRequired(false);
            
        builder.Property(x => x.Success)
            .IsRequired();
            
        builder.Property(x => x.Details)
            .IsRequired(false);
            
        builder.Property(x => x.Timestamp)
            .IsRequired();
            
        // Индексы для оптимизации запросов
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EventType);
        builder.HasIndex(x => x.Timestamp);
    }
}