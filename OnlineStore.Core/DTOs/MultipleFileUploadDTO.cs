using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class MultipleFileUploadDTO
{
    [Required]
    public List<Microsoft.AspNetCore.Http.IFormFile> Files { get; set; } = new();
    
    public bool IsPublic { get; set; } = false;
    
    public DateTime? ExpiresAt { get; set; }
}