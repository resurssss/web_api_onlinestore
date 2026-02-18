using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Configurations;

public class FavoriteItemConfiguration : IEntityTypeConfiguration<FavoriteItem>
{
    public void Configure(EntityTypeBuilder<FavoriteItem> builder)
    {
        builder.HasKey(f => f.Id);
        
        // Составной уникальный индекс
        builder.HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();
            
        // Связи
        builder.HasOne(f => f.User)
            .WithMany(u => u.FavoriteItems)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(f => f.Product)
            .WithMany(p => p.FavoriteItems)
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}