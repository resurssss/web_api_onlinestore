namespace OnlineStore.Core.DTOs;
public class CartItemCreateDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CartItemUpdateDto
{
    public int Quantity { get; set; }
}

public class CartItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // Упрощаем - вместо вложенного объекта
    public decimal ProductPrice { get; set; } // Упрощаем - вместо вложенного объекта
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

