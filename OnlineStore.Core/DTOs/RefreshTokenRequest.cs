using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}