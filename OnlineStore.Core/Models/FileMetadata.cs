using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models;

public class FileMetadata : BaseEntity
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;
    
    public long Size { get; set; }
    
    public int UploadedBy { get; set; }
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;
    
    [MaxLength(64)]
    public string Hash { get; set; } = string.Empty;
    
    public bool IsPublic { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
    public int DownloadCount { get; set; }
    
    // Свойства для изображений
    public int? Width { get; set; }
    
    public int? Height { get; set; }
    
    public string? DateTaken { get; set; }
    
    public string? CameraModel { get; set; }
    
    public string? Location { get; set; }
    
    public int? Orientation { get; set; }
    
    // Навигационные свойства
    public virtual User User { get; set; } = null!;
    
    // Soft delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}