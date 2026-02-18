using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(us => us.Id);
        
        builder.Property(us => us.RefreshTokenHash)
            .IsRequired();
            
        builder.Property(us => us.DeviceInfo)
            .HasMaxLength(255);
            
        builder.Property(us => us.IpAddress)
            .HasMaxLength(45); // Достаточно для IPv6
            
        // Связи
        builder.HasOne(us => us.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}