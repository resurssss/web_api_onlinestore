namespace OnlineStore.Core.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    
    // Навигационные свойства
    public Category Category { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<FavoriteItem> FavoriteItems { get; set; } = new List<FavoriteItem>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    
    // Методы для работы с запасами
    public bool CanBeOrdered(int quantity)
    {
        return IsActive && Stock >= quantity;
    }
    
    public void ReduceStock(int quantity)
    {
        if (quantity > 0 && Stock >= quantity)
        {
            Stock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public void IncreaseStock(int quantity)
    {
        if (quantity > 0)
        {
            Stock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public bool IsInStock()
    {
        return Stock > 0;
    }
}
