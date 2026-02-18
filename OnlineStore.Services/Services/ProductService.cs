using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using OnlineStore.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using System.Linq.Expressions;

namespace OnlineStore.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(OnlineStoreDbContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ----------------------
        // CRUD
        // ----------------------
        public async Task<ProductResponseDto> GetProductAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product by ID: {ProductId}", id);
            
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                throw new KeyNotFoundException($"Product with ID {id} not found");
            }
            
            _logger.LogInformation("Successfully retrieved product: {ProductName}", product.Name);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
            string? search = null,
            int? minPrice = null,
            int? maxPrice = null,
            bool? inStock = null,
            string? sortBy = null,
            bool descending = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting products with filters - Page: {Page}, PageSize: {PageSize}, Search: {Search}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, InStock: {InStock}, SortBy: {SortBy}, Descending: {Descending}",
                page, pageSize, search, minPrice, maxPrice, inStock, sortBy, descending);

            var query = _context.Products.AsQueryable();

            // ----------------------
            // Поиск (partial match, case-insensitive)
            // ----------------------
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search.ToLower()) ||
                    p.Description.ToLower().Contains(search.ToLower()));
            }

            // ----------------------
            // Фильтры
            // ----------------------
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (inStock.HasValue) query = query.Where(p => (p.Stock > 0) == inStock.Value);

            // ----------------------
            // Сортировка
            // ----------------------
            query = sortBy?.ToLower() switch
            {
                "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "stock" => descending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
                _ => query.OrderBy(p => p.Name) // default
            };

            // ----------------------
            // Пагинация
            // ----------------------
            var totalCount = await query.CountAsync(cancellationToken);
            var pagedItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} products for page {Page}", pagedItems.Count, page);

            return new PagedResultDto<ProductListItemDto>
            {
                Items = _mapper.Map<List<ProductListItemDto>>(pagedItems),
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        // ----------------------
        // Bulk операции
        // ----------------------
        public async Task<List<BulkOperationResultDto<ProductResponseDto>>> BulkCreateAsync(IEnumerable<ProductCreateDto> dtos, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk creating {Count} products", dtos.Count());

            var results = new List<BulkOperationResultDto<ProductResponseDto>>();

            foreach (var dto in dtos)
            {
                try
                {
                    // Проверка уникальности имени продукта
                    string productName = dto?.Name ?? string.Empty;
                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower(), cancellationToken);
                        
                    if (existingProduct != null)
                    {
                        _logger.LogWarning("Product creation failed - name already exists: {ProductName}", productName);
                        results.Add(new BulkOperationResultDto<ProductResponseDto>
                        {
                            ItemId = 0,
                            Success = false,
                            ErrorMessage = $"Product with name '{productName}' already exists"
                        });
                        continue;
                    }

                    var product = _mapper.Map<Product>(dto);
                    product.CreatedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;
                    
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync(cancellationToken);

                    results.Add(new BulkOperationResultDto<ProductResponseDto>
                    {
                        ItemId = product.Id,
                        Success = true,
                        Item = _mapper.Map<ProductResponseDto>(product)
                    });
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    _logger.LogError(ex, "Database error creating product with name: {ProductName}", dto?.Name ?? "Unknown");
                    results.Add(new BulkOperationResultDto<ProductResponseDto>
                    {
                        ItemId = 0,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            _logger.LogInformation("Bulk create completed. Success: {SuccessCount}, Failed: {FailedCount}",
                results.Count(r => r.Success), results.Count(r => !r.Success));

            return results;
        }

        // Bulk Update
        public async Task<List<BulkOperationResultDto<ProductResponseDto>>> BulkUpdateAsync(IEnumerable<(int Id, ProductUpdateDto Dto)> items, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk updating {Count} products", items.Count());
            
            var results = new List<BulkOperationResultDto<ProductResponseDto>>();

            foreach (var (id, dto) in items)
            {
                try
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                    
                    if (product == null)
                    {
                        _logger.LogWarning("Product not found for update with ID: {ProductId}", id);
                        results.Add(new BulkOperationResultDto<ProductResponseDto>
                        {
                            ItemId = id,
                            Success = false,
                            ErrorMessage = $"Product with ID {id} not found"
                        });
                        continue;
                    }

                    // Проверка уникальности имени продукта при обновлении
                    if (dto?.Name != null && dto.Name != product.Name)
                    {
                        var existingProduct = await _context.Products
                            .FirstOrDefaultAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id, cancellationToken);
                        
                        if (existingProduct != null)
                        {
                            _logger.LogWarning("Product update failed - name already exists: {ProductName}", dto.Name);
                            results.Add(new BulkOperationResultDto<ProductResponseDto>
                            {
                                ItemId = id,
                                Success = false,
                                ErrorMessage = $"Product with name '{dto.Name}' already exists"
                            });
                            continue;
                        }
                    }

                    _mapper.Map(dto, product);
                    product.UpdatedAt = DateTime.UtcNow;
                    
                    await _context.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogInformation("Successfully updated product {ProductName} (ID: {ProductId})",
                        product.Name, id);
                    
                    results.Add(new BulkOperationResultDto<ProductResponseDto>
                    {
                        ItemId = id,
                        Success = true,
                        Item = _mapper.Map<ProductResponseDto>(product)
                    });
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    _logger.LogError(ex, "Database error updating product with ID: {ProductId}", id);
                    results.Add(new BulkOperationResultDto<ProductResponseDto>
                    {
                        ItemId = id,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            _logger.LogInformation("Bulk update completed. Success: {SuccessCount}, Failed: {FailedCount}",
                results.Count(r => r.Success), results.Count(r => !r.Success));

            return results;
        }

        // Bulk Delete
        public async Task<List<BulkOperationResultDto<object>>> BulkDeleteAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk deleting {Count} products", ids.Count());
            
            var results = new List<BulkOperationResultDto<object>>();

            foreach (var id in ids)
            {
                try
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                    var deleted = product != null;
                    
                    if (deleted && product != null)
                    {
                        _context.Products.Remove(product);
                        await _context.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
                    }
                    else
                    {
                        _logger.LogWarning("Product not found for deletion with ID: {ProductId}", id);
                    }
                    
                    results.Add(new BulkOperationResultDto<object>
                    {
                        ItemId = id,
                        Success = deleted,
                        ErrorMessage = deleted ? null : "Item not found"
                    });
                }
                catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
                {
                    _logger.LogError(ex, "Database error deleting product with ID: {ProductId}", id);
                    results.Add(new BulkOperationResultDto<object>
                    {
                        ItemId = id,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            _logger.LogInformation("Bulk delete completed. Success: {SuccessCount}, Failed: {FailedCount}",
                results.Count(r => r.Success), results.Count(r => !r.Success));

            return results;
        }
        
        // ----------------------
        // Поиск продуктов
        // ----------------------
        public async Task<IEnumerable<ProductListItemDto>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
            
            var products = await _context.Products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync(cancellationToken);
                
            _logger.LogInformation("Found {Count} products matching search term: {SearchTerm}", products.Count, searchTerm);
            return _mapper.Map<IEnumerable<ProductListItemDto>>(products);
        }
        
        // ----------------------
        // Фильтрация по диапазону цен
        // ----------------------
        public async Task<IEnumerable<ProductListItemDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
            
            var products = await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync(cancellationToken);
                
            _logger.LogInformation("Found {Count} products in price range: {MinPrice} - {MaxPrice}", products.Count, minPrice, maxPrice);
            return _mapper.Map<IEnumerable<ProductListItemDto>>(products);
        }
        
        // ----------------------
        // Статистика по продуктам
        // ----------------------
        public async Task<ProductStatisticsDto> GetProductStatisticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product statistics");
            
            var products = await _context.Products.ToListAsync(cancellationToken);
            
            var statistics = new ProductStatisticsDto
            {
                TotalProducts = products.Count,
                TotalInStock = products.Count(p => p.IsInStock()),
                TotalOutOfStock = products.Count(p => !p.IsInStock()),
                AveragePrice = products.Any() ? products.Average(p => p.Price) : 0,
                MinPrice = products.Any() ? products.Min(p => p.Price) : 0,
                MaxPrice = products.Any() ? products.Max(p => p.Price) : 0
            };
            
            _logger.LogInformation("Successfully retrieved product statistics: {@Statistics}", statistics);
            return statistics;
        }
        
        // ----------------------
        // Работа с изображениями продуктов
        // ----------------------
        public async Task<List<ProductImageResponseDto>> GetProductImagesAsync(int productId, CancellationToken cancellationToken = default)
        {
            // Проверка существования продукта
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            
            // Получение изображений продукта с сортировкой по Order
            var images = await _context.ProductImages
                .Include(pi => pi.File)
                .Where(pi => pi.ProductId == productId)
                .OrderBy(pi => pi.Order)
                .ToListAsync(cancellationToken);
                
            // Преобразование в DTO
            var imageDtos = new List<ProductImageResponseDto>();
            
            foreach (var image in images)
            {
                var fileDto = _mapper.Map<FileMetadataDTO>(image.File);
                
                // Формирование URL для файла и thumbnail
                var baseUrl = "/api/files"; // Базовый URL для файлов
                var fileUrl = $"{baseUrl}/{image.FileId}";
                var thumbnailUrl = $"{baseUrl}/{image.FileId}/thumbnail";
                
                imageDtos.Add(new ProductImageResponseDto
                {
                    Id = image.Id,
                    ProductId = image.ProductId,
                    FileId = image.FileId,
                    IsMain = image.IsMain,
                    Order = image.Order,
                    File = fileDto,
                    FileUrl = fileUrl,
                    ThumbnailUrl = thumbnailUrl
                });
            }
            
            return imageDtos;
        }
        
        public async Task<ProductImageResponseDto> AddProductImageAsync(int productId, ProductImageCreateDto dto, CancellationToken cancellationToken = default)
        {
            // Проверка существования продукта
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            
            // Если FileId не предоставлен, выбрасываем исключение
            if (!dto.FileId.HasValue)
            {
                throw new ArgumentException("FileId is required");
            }
            
            // Проверка существования файла
            var file = await _context.FileMetadata
                .FirstOrDefaultAsync(f => f.Id == dto.FileId, cancellationToken);
                
            if (file == null)
            {
                throw new KeyNotFoundException($"File with ID {dto.FileId} not found");
            }
            
            // Проверка, что файл является изображением
            if (!file.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("File must be an image");
            }
            
            // Создание записи ProductImage
            var productImage = new ProductImage
            {
                ProductId = productId,
                FileId = dto.FileId.Value,
                IsMain = dto.IsMain,
                Order = dto.Order,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Получение полной информации о файле
            var fileDto = _mapper.Map<FileMetadataDTO>(file);
            
            // Формирование URL для файла и thumbnail
            var baseUrl = "/api/files"; // Базовый URL для файлов
            var fileUrl = $"{baseUrl}/{file.Id}";
            var thumbnailUrl = $"{baseUrl}/{file.Id}/thumbnail";
            
            return new ProductImageResponseDto
            {
                Id = productImage.Id,
                ProductId = productImage.ProductId,
                FileId = productImage.FileId,
                IsMain = productImage.IsMain,
                Order = productImage.Order,
                File = fileDto,
                FileUrl = fileUrl,
                ThumbnailUrl = thumbnailUrl
            };
        }
        
        public async Task RemoveProductImageAsync(int productId, int imageId, CancellationToken cancellationToken = default)
        {
            // Проверка существования продукта
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            
            // Поиск изображения
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == imageId && pi.ProductId == productId, cancellationToken);
                
            if (image == null)
            {
                throw new KeyNotFoundException($"Image with ID {imageId} not found for product {productId}");
            }
            
            // Удаление записи
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        // Добавляем метод для получения продукта с изображениями
        public async Task<ProductResponseDto> GetProductWithImagesAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product with images by ID: {ProductId}", id);
            
            var product = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.File)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                throw new KeyNotFoundException($"Product with ID {id} not found");
            }
            
            var dto = _mapper.Map<ProductResponseDto>(product);
            
            // Преобразование изображений в DTO
            var imageDtos = new List<ProductImageResponseDto>();
            var baseUrl = "/api/files"; // Базовый URL для файлов
            
            foreach (var image in product.ProductImages.OrderBy(pi => pi.Order))
            {
                var fileDto = _mapper.Map<FileMetadataDTO>(image.File);
                var fileUrl = $"{baseUrl}/{image.FileId}";
                var thumbnailUrl = $"{baseUrl}/{image.FileId}/thumbnail";
                
                imageDtos.Add(new ProductImageResponseDto
                {
                    Id = image.Id,
                    ProductId = image.ProductId,
                    FileId = image.FileId,
                    IsMain = image.IsMain,
                    Order = image.Order,
                    File = fileDto,
                    FileUrl = fileUrl,
                    ThumbnailUrl = thumbnailUrl
                });
            }
            
            dto.Images = imageDtos;
            _logger.LogInformation("Successfully retrieved product with images: {ProductName}", product.Name);
            return dto;
        }
    }
}