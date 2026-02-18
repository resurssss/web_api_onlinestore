using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(c => c.DiscountPercent)
            .IsRequired()
            .HasPrecision(5, 2);
            
        builder.Property(c => c.ExpirationDate)
            .IsRequired();
            
        builder.Property(c => c.TimesUsed)
            .HasDefaultValue(0);
            
        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);
            
        builder.HasIndex(c => c.Code)
            .IsUnique();
    }
}