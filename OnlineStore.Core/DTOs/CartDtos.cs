namespace OnlineStore.Core.DTOs;
public class CartResponseDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;

    public List<CartItemResponseDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public decimal TotalPrice { get; set; }

    public CouponResponseDto? AppliedCoupon { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}