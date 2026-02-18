using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Core;
using OnlineStore.Core.Models;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(OnlineStoreDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Создание заказа с использованием транзакций
        public async Task<Order?> CreateOrderAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Создание заказа для пользователя {UserId} с {ItemCount} элементами", userId, cartItems.Count);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
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

                // Фиксируем транзакцию
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Заказ {OrderId} успешно создан для пользователя {UserId}", order.Id, userId);
                return order;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                // Откатываем транзакцию при ошибке
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database error при создании заказа для пользователя {UserId}", userId);
                throw;
            }
        }

        // Создание заказа с использованием Savepoint
        public async Task<Order?> CreateOrderWithSavepointAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Создание заказа с Savepoint для пользователя {UserId}", userId);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
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

                // Создаем Savepoint перед созданием элементов заказа
                await transaction.CreateSavepointAsync("BeforeOrderItems", cancellationToken);
                string savepointName = "BeforeOrderItems";

                try
                {
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
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    // Откатываем к Savepoint при ошибке создания элементов заказа
                    await transaction.RollbackToSavepointAsync(savepointName, cancellationToken);
                    _logger.LogWarning(ex, "Откат к Savepoint при создании элементов заказа для пользователя {UserId}", userId);

                    // Повторно создаем элементы заказа (в реальном приложении здесь может быть другая логика)
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
                }

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

                // Фиксируем транзакцию
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Заказ {OrderId} успешно создан с Savepoint для пользователя {UserId}", order.Id, userId);
                return order;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                // Откатываем транзакцию при ошибке
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database error при создании заказа с Savepoint для пользователя {UserId}", userId);
                throw;
            }
        }

        // Создание заказа с пользовательским уровнем изоляции
        public async Task<Order?> CreateOrderWithIsolationLevelAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Создание заказа с пользовательским уровнем изоляции для пользователя {UserId}", userId);

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
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

                // Фиксируем транзакцию
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Заказ {OrderId} успешно создан с пользовательским уровнем изоляции для пользователя {UserId}", order.Id, userId);
                return order;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                // Откатываем транзакцию при ошибке
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database error при создании заказа с пользовательским уровнем изоляции для пользователя {UserId}", userId);
                throw;
            }
        }
    }
}