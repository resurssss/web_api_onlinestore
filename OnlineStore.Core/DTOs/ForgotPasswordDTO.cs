using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
    }
}