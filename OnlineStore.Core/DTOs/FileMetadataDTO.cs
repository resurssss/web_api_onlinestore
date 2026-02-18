using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class FileMetadataDTO
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
    
    public DateTime UploadedAt { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public bool IsPublic { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
    public int DownloadCount { get; set; }
    
    [MaxLength(64)]
    public string Hash { get; set; } = string.Empty;
    
    // Свойства для изображений
    public int? Width { get; set; }
    
    public int? Height { get; set; }
    
    public string? DateTaken { get; set; }
    
    public string? CameraModel { get; set; }
    
    public string? Location { get; set; }
    
    public int? Orientation { get; set; }
}