using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Core;
using OnlineStore.Core.Models;
using OnlineStore.Core.Interfaces;
using System.Linq;

namespace OnlineStore.Services.Services
{
    public class TestOrderService : IOrderService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<TestOrderService> _logger;

        public TestOrderService(OnlineStoreDbContext context, IProductService productService, ILogger<TestOrderService> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        // Создание заказа без использования транзакций
        public async Task<Order?> CreateOrderAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Создание заказа для пользователя {UserId} с {ItemCount} элементами", userId, cartItems.Count);

            try
            {
                // Проверяем существование всех продуктов
                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products.FindAsync(new object[] { cartItem.ProductId }, cancellationToken);
                    if (product == null)
                    {
                        _logger.LogWarning("Product {ProductId} not found when creating order for user {UserId}", cartItem.ProductId, userId);
                        throw new InvalidOperationException($"Product with ID {cartItem.ProductId} not found");
                    }
                }

                // Создаем заказ
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Created",
                    TotalAmount = cartItems.Sum(item => item.TotalPrice)
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                // Создаем элементы заказа
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Обновляем запасы товаров
                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products.FindAsync(new object[] { cartItem.ProductId }, cancellationToken);
                    if (product != null)
                    {
                        product.ReduceStock(cartItem.Quantity);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Заказ {OrderId} успешно создан для пользователя {UserId}", order.Id, userId);
                return order;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error при создании заказа для пользователя {UserId}", userId);
                throw;
            }
        }

        // Создание заказа с использованием Savepoint (без транзакций)
        public async Task<Order?> CreateOrderWithSavepointAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            return await CreateOrderAsync(userId, cartItems, cancellationToken);
        }

        // Создание заказа с пользовательским уровнем изоляции (без транзакций)
        public async Task<Order?> CreateOrderWithIsolationLevelAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            return await CreateOrderAsync(userId, cartItems, cancellationToken);
        }
    }
}