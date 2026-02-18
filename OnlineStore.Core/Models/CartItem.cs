namespace OnlineStore.Core.Models;

public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }

    // Цена за единицу на момент добавления (persisted)
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    // Навигационные свойства (могут быть null после десериализации)
    public virtual Cart? Cart { get; set; }
    public virtual Product? Product { get; set; }

    // Безопасный TotalPrice — не использует Product напрямую
    public decimal TotalPrice => UnitPrice * Quantity;

    public void IncreaseQuantity(int amount = 1)
    {
        if (amount <= 0) return;
        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseQuantity(int amount = 1)
    {
        if (amount <= 0) return;
        Quantity = Math.Max(0, Quantity - amount);
        UpdatedAt = DateTime.UtcNow;
    }
}
