namespace OnlineStore.Core.DTOs;

public class CouponCreateDto
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
}

public class CouponUpdateDto
{
    public string? Code { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
}

public class CouponResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
    public int TimesUsed { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CouponListItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public bool IsActive { get; set; }
}