namespace OnlineStore.Core.Models;

public class FavoriteItem : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    
    // Навигационные свойства
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}