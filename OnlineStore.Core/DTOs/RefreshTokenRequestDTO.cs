using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class RefreshTokenRequestDTO
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}