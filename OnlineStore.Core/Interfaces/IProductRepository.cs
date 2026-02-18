using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByStockStatusAsync(bool inStock, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<bool> UpdateStockAsync(int productId, int quantityChange, CancellationToken cancellationToken = default);
    }
}