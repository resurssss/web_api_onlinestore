using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.DTOs
{
    public class ConfirmEmailDTO
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}