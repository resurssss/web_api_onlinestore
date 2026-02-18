namespace OnlineStore.Core.Models;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int? UsageLimit { get; set; }
    public int TimesUsed { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Метод для проверки активности купона
    public bool IsActiveCoupon()
    {
        return IsActive && DateTime.UtcNow <= ExpirationDate && (UsageLimit == null || TimesUsed < UsageLimit);
    }
}