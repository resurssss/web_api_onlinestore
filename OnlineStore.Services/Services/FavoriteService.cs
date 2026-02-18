using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using OnlineStore.Core;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Services.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(OnlineStoreDbContext context,
                               IMapper mapper,
                               ILogger<FavoriteService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FavoriteListItemDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting favorites for user {UserId}", userId);

            var favorites = await _context.FavoriteItems
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} favorites for user {UserId}", favorites.Count, userId);
            return _mapper.Map<IEnumerable<FavoriteListItemDto>>(favorites);
        }
        
        public async Task<PagedResultDto<FavoriteListItemDto>> GetUserFavoritesAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged favorites for user {UserId} - Page: {Page}, PageSize: {PageSize}", userId, page, pageSize);
            
            var query = _context.FavoriteItems
                .Include(f => f.Product)
                .Where(f => f.UserId == userId);
            
            var totalCount = await query.CountAsync(cancellationToken);
            var favorites = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            
            _logger.LogInformation("Retrieved {Count} favorites for user {UserId} - Page: {Page}", favorites.Count, userId, page);
            
            return new PagedResultDto<FavoriteListItemDto>
            {
                Items = _mapper.Map<IEnumerable<FavoriteListItemDto>>(favorites),
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<FavoriteResponseDto> AddFavoriteAsync(FavoriteCreateDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding product {ProductId} to favorites for user {UserId}", dto.ProductId, dto.UserId);
            
            // Проверяем существование продукта
            var product = await _context.Products.FindAsync(new object[] { dto.ProductId }, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found when adding to favorites for user {UserId}", dto.ProductId, dto.UserId);
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");
            }

            // Проверяем, не добавлен ли уже продукт в избранное
            var exists = await _context.FavoriteItems
                .AnyAsync(f => f.UserId == dto.UserId && f.ProductId == dto.ProductId, cancellationToken);
                
            if (exists)
            {
                _logger.LogWarning("Product {ProductId} already in favorites for user {UserId}", dto.ProductId, dto.UserId);
                throw new InvalidOperationException($"Product {dto.ProductId} already in favorites for user {dto.UserId}");
            }

            var favorite = new FavoriteItem
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Product = product,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.FavoriteItems.Add(favorite);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully added product {ProductId} to favorites for user {UserId}", dto.ProductId, dto.UserId);
            return _mapper.Map<FavoriteResponseDto>(favorite);
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing product {ProductId} from favorites for user {UserId}", productId, userId);
            
            var favorite = await _context.FavoriteItems
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId, cancellationToken);
            
            if (favorite != null)
            {
                _context.FavoriteItems.Remove(favorite);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully removed product {ProductId} from favorites for user {UserId}", productId, userId);
                return true;
            }
            else
            {
                _logger.LogWarning("Product {ProductId} not found in favorites for user {UserId}", productId, userId);
                return false;
            }
        }
    }
}
