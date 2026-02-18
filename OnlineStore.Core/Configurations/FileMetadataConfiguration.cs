using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Configurations;

public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(f => f.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(f => f.Path)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(f => f.Hash)
            .IsRequired()
            .HasMaxLength(64);
            
        builder.Property(f => f.UploadedAt)
            .IsRequired();
            
        builder.Property(f => f.Size)
            .IsRequired();
            
        builder.Property(f => f.IsPublic)
            .IsRequired();
            
        builder.Property(f => f.DownloadCount)
            .IsRequired();
            
        // Связь с пользователем
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UploadedBy)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}