using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int cartId, CancellationToken cancellationToken = default);
        Task<CartResponseDto> AddItemAsync(int cartId, int productId, int quantity, CancellationToken cancellationToken = default);
        Task<CartResponseDto> UpdateItemQuantityAsync(int cartId, int productId, int quantity, CancellationToken cancellationToken = default);
        Task RemoveItemAsync(int cartId, int productId, CancellationToken cancellationToken = default);
        Task<CartResponseDto> ClearCartAsync(int cartId, CancellationToken cancellationToken = default);
        Task<CartResponseDto> ApplyCouponAsync(int cartId, string code, CancellationToken cancellationToken = default);
        
        // Bulk операции
        Task<List<(int? ProductId, bool Success, string? Error)>> BulkAddItemsAsync(int cartId, List<CartItemCreateDto> items, CancellationToken cancellationToken = default);
        Task<List<(int ProductId, bool Success, string? Error)>> BulkRemoveItemsAsync(int cartId, List<int> productIds, CancellationToken cancellationToken = default);
    }
}