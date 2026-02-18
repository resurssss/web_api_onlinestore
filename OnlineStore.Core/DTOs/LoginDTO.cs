using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs;

public class LoginDTO
{
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}