namespace OnlineStore.Core.DTOs;

public class FavoriteCreateDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
}

public class FavoriteResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    // Вложенные данные - продукт
    public ProductListItemDto Product { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class FavoriteListItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
}