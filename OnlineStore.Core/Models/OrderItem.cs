namespace OnlineStore.Core.Models;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    // Навигационные свойства
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}