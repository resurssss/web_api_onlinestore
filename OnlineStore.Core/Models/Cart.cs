using System.Linq;

namespace OnlineStore.Core.Models;

public class Cart : BaseEntity
{
    public int? UserId { get; set; } // Может быть null для анонимных пользователей
    public string SessionId { get; set; } = string.Empty; // Для корзин без пользователя

    // Навигационные свойства
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public virtual Coupon? AppliedCoupon { get; set; }
    public int? AppliedCouponId { get; set; }

    // Добавляем товар в корзину — сохраняем UnitPrice из Product.Price
    public void AddItem(Product product, int quantity)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) return;

        var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            Items.Add(new CartItem
            {
                Id = 0,
                CartId = this.Id,
                ProductId = product.Id,
                Product = product,
                UnitPrice = product.Price,        // <- сохраняем цену на момент добавления
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(int productId)
    {
        var item = Items.FirstOrDefault(item => item.ProductId == productId);
        if (item != null)
        {
            Items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateItemQuantity(int productId, int quantity)
    {
        if (quantity <= 0) return;

        var item = Items.FirstOrDefault(item => item.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            item.UpdatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // Безопасный подсчёт общей суммы
    public decimal TotalPrice
    {
        get
        {
            // защита: если Items == null, считаем 0
            var total = (Items ?? Enumerable.Empty<CartItem>()).Sum(item => item?.TotalPrice ?? 0m);

            if (AppliedCoupon != null && AppliedCoupon.IsActiveCoupon())
            {
                total -= total * (AppliedCoupon.DiscountPercent / 100m);
            }

            return total;
        }
    }

    public int TotalItems => (Items ?? Enumerable.Empty<CartItem>()).Sum(item => item?.Quantity ?? 0);

    public bool ApplyCoupon(Coupon coupon)
    {
        if (coupon != null && coupon.IsActiveCoupon() && TotalPrice > 0)
        {
            AppliedCoupon = coupon;
            AppliedCouponId = coupon.Id;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public void RemoveCoupon()
    {
        AppliedCoupon = null;
        AppliedCouponId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmpty() => (Items == null) || !Items.Any();

    public void Clear()
    {
        Items?.Clear();
        RemoveCoupon();
        UpdatedAt = DateTime.UtcNow;
    }
}
