namespace OnlineStore.Core.DTOs;

// ТОЛЬКО необходимые поля для создания
public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// NULLABLE поля для обновления
public class ProductUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
}

// Полные данные + вложенные для ответа
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsInStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // Вложенные данные - отзывы и рейтинг
    public List<ReviewResponseDto> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    
    // Изображения продукта
    public List<ProductImageResponseDto> Images { get; set; } = new();
}

// Упрощенная версия для списков
public class ProductListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsInStock { get; set; }
}

// Статистика по продуктам
public class ProductStatisticsDto
{
    public int TotalProducts { get; set; }
    public int TotalInStock { get; set; }
    public int TotalOutOfStock { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
}