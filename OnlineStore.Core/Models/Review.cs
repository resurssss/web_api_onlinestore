namespace OnlineStore.Core.Models;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public int? UserId { get; set; }
    public string Author { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsVerifiedPurchase { get; set; }
    
    // Навигационные свойства
    public Product Product { get; set; } = null!;
    public User? User { get; set; }
}