using Microsoft.Extensions.Logging;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OnlineStore.Core;

namespace OnlineStore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OnlineStoreDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(OnlineStoreDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product by ID: {ProductId}", id);
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all products");
            return await _context.Products.ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product by name: {ProductName}", name);
            return await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Checking if product exists by name: {ProductName}", name);
            return await _context.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByStockStatusAsync(bool inStock, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting products by stock status: {InStock}", inStock);
            return await _context.Products
                .Where(p => (p.Stock > 0) == inStock)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting active products");
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantityChange, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating stock for product ID: {ProductId} with change: {QuantityChange}", productId, quantityChange);
            
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
                if (product == null)
                {
                    _logger.LogWarning("Product not found for stock update with ID: {ProductId}", productId);
                    return false;
                }
                
                product.Stock += quantityChange;
                product.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully updated stock for product {ProductName} (ID: {ProductId})", product.Name, productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product with ID: {ProductId}", productId);
                return false;
            }
        }
    }
}