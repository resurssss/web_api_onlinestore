using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core;
using OnlineStore.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using System.Linq.Expressions;

namespace OnlineStore.Services.Services
{
    public class CartService : ICartService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            OnlineStoreDbContext context,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartResponseDto> GetCartAsync(int cartId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting cart with ID: {CartId}", cartId);
            
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

            if (cart == null)
            {
                _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                throw new KeyNotFoundException($"Cart with ID {cartId} not found");
            }
            
            _logger.LogInformation("Successfully retrieved cart with {ItemCount} items", cart.Items.Count);
            return _mapper.Map<CartResponseDto>(cart);
        }

        public async Task<CartResponseDto> AddItemAsync(int cartId, int productId, int quantity, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding item to cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                cartId, productId, quantity);
            
            try
            {
                Cart cart;
                if (cartId == 0)
                {
                    cart = new Cart { SessionId = Guid.NewGuid().ToString() };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync(cancellationToken);
                    cartId = cart.Id;
                }
                else
                {
                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken) ?? throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                        
                    if (cart == null)
                    {
                        _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                        throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                    }
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", productId);
                    throw new KeyNotFoundException($"Product with ID {productId} not found");
                }
    
                if (!product.CanBeOrdered(quantity))
                {
                    _logger.LogWarning("Not enough stock for product {ProductName} (ID: {ProductId})",
                        product.Name, product.Id);
                    throw new InvalidOperationException("Not enough stock");
                }

                // Проверяем, есть ли уже такой товар в корзине
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.CartItems.Add(cartItem);
                }

                product.ReduceStock(quantity);
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully added item to cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                    cart.Id, productId, quantity);

                return _mapper.Map<CartResponseDto>(cart);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error adding item to cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                    cartId, productId, quantity);
                throw;
            }
        }

        public async Task<CartResponseDto> UpdateItemQuantityAsync(int cartId, int productId, int quantity, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating item quantity in cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                cartId, productId, quantity);
            
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
    
                if (cart == null)
                {
                    _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                    throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    _logger.LogWarning("Item not found in cart - CartId: {CartId}, ProductId: {ProductId}", cartId, productId);
                    throw new KeyNotFoundException($"Item with ProductId {productId} not found in cart {cartId}");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", productId);
                    throw new KeyNotFoundException($"Product with ID {productId} not found");
                }

                // Восстанавливаем запас товара
                product.IncreaseStock(item.Quantity);

                // Проверяем, достаточно ли товара для нового количества
                if (quantity > 0 && !product.CanBeOrdered(quantity))
                {
                    // Возвращаем запас обратно
                    product.ReduceStock(item.Quantity);
                    _logger.LogWarning("Not enough stock for product {ProductName} (ID: {ProductId})",
                        product.Name, product.Id);
                    throw new InvalidOperationException("Not enough stock");
                }

                if (quantity > 0)
                {
                    item.Quantity = quantity;
                    item.UpdatedAt = DateTime.UtcNow;
                    product.ReduceStock(quantity);
                }
                else
                {
                    _context.CartItems.Remove(item);
                }

                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated item quantity in cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                    cartId, productId, quantity);

                return _mapper.Map<CartResponseDto>(cart);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error updating item quantity in cart - CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
                    cartId, productId, quantity);
                throw;
            }
        }

        public async Task RemoveItemAsync(int cartId, int productId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing item from cart - CartId: {CartId}, ProductId: {ProductId}",
                cartId, productId);
            
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken) ?? throw new KeyNotFoundException($"Cart with ID {cartId} not found");
    
                if (cart == null)
                {
                    _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                    throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    
                    if (product == null)
                    {
                        _logger.LogWarning("Product not found with ID: {ProductId}", productId);
                        throw new KeyNotFoundException($"Product with ID {productId} not found");
                    }
                    
                    product.IncreaseStock(item.Quantity);

                    _context.CartItems.Remove(item);
                    cart.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogInformation("Successfully removed item from cart - CartId: {CartId}, ProductId: {ProductId}",
                        cartId, productId);
                }
                else
                {
                    _logger.LogWarning("Item not found in cart - CartId: {CartId}, ProductId: {ProductId}", cartId, productId);
                }
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error removing item from cart - CartId: {CartId}, ProductId: {ProductId}",
                    cartId, productId);
                throw;
            }
        }

        public async Task<CartResponseDto> ClearCartAsync(int cartId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Clearing cart with ID: {CartId}", cartId);
            
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken) ?? throw new KeyNotFoundException($"Cart with ID {cartId} not found");
    
                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for clearing with ID: {CartId}", cartId);
                    throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                }
                
                // Восстанавливаем запасы товаров
                foreach (var item in cart.Items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
                    if (product != null)
                    {
                        product.IncreaseStock(item.Quantity);
                    }
                }
                
                _context.CartItems.RemoveRange(cart.Items);
                cart.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Successfully cleared cart with ID: {CartId}", cartId);
                
                return _mapper.Map<CartResponseDto>(cart);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error clearing cart with ID: {CartId}", cartId);
                throw;
            }
        }

        public async Task<CartResponseDto> ApplyCouponAsync(int cartId, string code, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Applying coupon to cart - CartId: {CartId}, CouponCode: {CouponCode}",
                cartId, code);
            
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken) ?? throw new KeyNotFoundException($"Cart with ID {cartId} not found");
    
                if (cart == null)
                {
                    _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                    throw new KeyNotFoundException($"Cart with ID {cartId} not found");
                }
                
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && c.IsActive, cancellationToken);
    
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon not found with code: {CouponCode}", code);
                    throw new KeyNotFoundException($"Coupon with code {code} not found");
                }

                // Проверяем, не истек ли купон
                if (coupon.ExpirationDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Coupon expired: {CouponCode}", code);
                    throw new InvalidOperationException($"Coupon {code} has expired");
                }

                // Проверяем, не использован ли купон максимальное количество раз
                if (coupon.UsageLimit.HasValue && coupon.TimesUsed >= coupon.UsageLimit.Value)
                {
                    _logger.LogWarning("Coupon usage limit reached: {CouponCode}", code);
                    throw new InvalidOperationException($"Coupon {code} has reached its usage limit");
                }

                cart.AppliedCouponId = coupon.Id;
                cart.UpdatedAt = DateTime.UtcNow;
                
                // Увеличиваем счетчик использования купона
                coupon.TimesUsed++;
                coupon.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Successfully applied coupon to cart - CartId: {CartId}, CouponCode: {CouponCode}",
                    cartId, code);

                return _mapper.Map<CartResponseDto>(cart);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error applying coupon to cart - CartId: {CartId}, CouponCode: {CouponCode}",
                    cartId, code);
                throw;
            }
        }

        // --- Bulk операции ---
        public async Task<List<(int? ProductId, bool Success, string? Error)>> BulkAddItemsAsync(int cartId, List<CartItemCreateDto> items, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk adding items to cart - CartId: {CartId}, ItemCount: {ItemCount}",
                cartId, items.Count);
            
            var results = new List<(int?, bool, string?)>();
            foreach (var item in items)
            {
                try
                {
                    var cartDto = await AddItemAsync(cartId, item.ProductId, item.Quantity, cancellationToken);
                    results.Add((item.ProductId, true, null));
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    _logger.LogError(ex, "Database error adding item to cart - CartId: {CartId}, ProductId: {ProductId}",
                        cartId, item.ProductId);
                    results.Add((item.ProductId, false, ex.Message));
                }
            }
            
            _logger.LogInformation("Bulk add items completed - CartId: {CartId}, Success: {SuccessCount}, Failed: {FailedCount}",
                cartId, results.Count(r => r.Item2), results.Count(r => !r.Item2));

            return results;
        }

        public async Task<List<(int ProductId, bool Success, string? Error)>> BulkRemoveItemsAsync(int cartId, List<int> productIds, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk removing items from cart - CartId: {CartId}, ItemCount: {ItemCount}",
                cartId, productIds.Count);

            var results = new List<(int, bool, string?)>();
            foreach (var productId in productIds)
            {
                try
                {
                    await RemoveItemAsync(cartId, productId, cancellationToken);
                    results.Add((productId, true, null));
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    _logger.LogError(ex, "Database error removing item from cart - CartId: {CartId}, ProductId: {ProductId}",
                        cartId, productId);
                    results.Add((productId, false, ex.Message));
                }
            }
            
            _logger.LogInformation("Bulk remove items completed - CartId: {CartId}, Success: {SuccessCount}, Failed: {FailedCount}",
                cartId, results.Count(r => r.Item2), results.Count(r => !r.Item2));

            return results;
        }
    }
}