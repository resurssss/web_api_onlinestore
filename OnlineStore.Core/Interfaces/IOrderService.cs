using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IOrderService
    {
        // Создание заказа с использованием транзакций
        Task<Order?> CreateOrderAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default);
        
        // Создание заказа с использованием Savepoint
        Task<Order?> CreateOrderWithSavepointAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default);
        
        // Создание заказа с пользовательским уровнем изоляции
        Task<Order?> CreateOrderWithIsolationLevelAsync(int userId, List<CartItem> cartItems, CancellationToken cancellationToken = default);
    }
}