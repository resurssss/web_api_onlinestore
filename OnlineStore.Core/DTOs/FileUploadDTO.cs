using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class FileUploadDTO
{
    [Required]
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
    
    public bool IsPublic { get; set; } = false;
    
    public DateTime? ExpiresAt { get; set; }
}