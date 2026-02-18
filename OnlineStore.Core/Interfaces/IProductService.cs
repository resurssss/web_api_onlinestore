using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IProductService
    {
        // CRUD
        Task<ProductResponseDto> GetProductAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
            string? search = null,
            int? minPrice = null,
            int? maxPrice = null,
            bool? inStock = null,
            string? sortBy = null,
            bool descending = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        
        // Bulk операции
        Task<List<BulkOperationResultDto<ProductResponseDto>>> BulkCreateAsync(IEnumerable<ProductCreateDto> dtos, CancellationToken cancellationToken = default);
        Task<List<BulkOperationResultDto<ProductResponseDto>>> BulkUpdateAsync(IEnumerable<(int Id, ProductUpdateDto Dto)> items, CancellationToken cancellationToken = default);
        Task<List<BulkOperationResultDto<object>>> BulkDeleteAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        
        // Поиск продуктов
        Task<IEnumerable<ProductListItemDto>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
        
        // Фильтрация по диапазону цен
        Task<IEnumerable<ProductListItemDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        
        // Статистика по продуктам
        Task<ProductStatisticsDto> GetProductStatisticsAsync(CancellationToken cancellationToken = default);
        
        // Работа с изображениями продуктов
        Task<List<ProductImageResponseDto>> GetProductImagesAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductImageResponseDto> AddProductImageAsync(int productId, ProductImageCreateDto dto, CancellationToken cancellationToken = default);
        Task RemoveProductImageAsync(int productId, int imageId, CancellationToken cancellationToken = default);
        Task<ProductResponseDto> GetProductWithImagesAsync(int id, CancellationToken cancellationToken = default);
    }
}