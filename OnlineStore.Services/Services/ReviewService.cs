using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core;
using System.Linq.Expressions;

namespace OnlineStore.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(OnlineStoreDbContext context, IMapper mapper, ILogger<ReviewService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetReviewsAsync(int? productId = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting reviews, ProductId: {ProductId}", productId);
            
            try
            {
                var query = _context.Reviews.AsQueryable();

                if (productId.HasValue)
                    query = query.Where(r => r.ProductId == productId.Value);

                var reviews = await query.ToListAsync(cancellationToken);
                
                _logger.LogInformation("Retrieved {Count} reviews, ProductId: {ProductId}", reviews.Count(), productId);
                return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error getting reviews, ProductId: {ProductId}", productId);
                throw;
            }
        }
        
        public async Task<PagedResultDto<ReviewResponseDto>> GetReviewsAsync(int productId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged reviews for product {ProductId} - Page: {Page}, PageSize: {PageSize}", productId, page, pageSize);
            
            try
            {
                var query = _context.Reviews.Where(r => r.ProductId == productId);
                var totalCount = await query.CountAsync(cancellationToken);
                var pagedReviews = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
                
                _logger.LogInformation("Retrieved {Count} reviews for product {ProductId} - Page: {Page}", pagedReviews.Count, productId, page);
                
                return new PagedResultDto<ReviewResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ReviewResponseDto>>(pagedReviews),
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error getting paged reviews for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<ReviewResponseDto> AddReviewAsync(ReviewCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                _logger.LogWarning("Review DTO is null");
                throw new ArgumentNullException(nameof(dto));
            }
            
            _logger.LogInformation("Adding review for product {ProductId} by user {Author}", dto.ProductId, dto.Author ?? "Unknown");
            
            // Проверяем существование продукта
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId, cancellationToken);
            
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found when adding review", dto.ProductId);
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");
            }

            // Проверяем, не оставлял ли уже пользователь отзыв на этот продукт
            var hasReviewed = await _context.Reviews.AnyAsync(r => r.ProductId == dto.ProductId &&
                                             (r.Author ?? string.Empty).Equals(dto.Author ?? string.Empty, StringComparison.OrdinalIgnoreCase),
                                             cancellationToken);
            
            if (hasReviewed)
            {
                _logger.LogWarning("User {Author} has already reviewed product {ProductId}", dto.Author ?? "Unknown", dto.ProductId);
                throw new InvalidOperationException($"User {dto.Author ?? "Unknown"} has already reviewed product {dto.ProductId}");
            }

            var review = _mapper.Map<Review>(dto);
            review.CreatedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully added review for product {ProductId} by user {Author}", dto.ProductId, dto.Author ?? "Unknown");

            return _mapper.Map<ReviewResponseDto>(review);
        }

        public async Task<(double rating, int count)> GetProductRatingAsync(int productId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting rating for product {ProductId}", productId);
            
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.ProductId == productId)
                    .ToListAsync(cancellationToken);
                
                var count = reviews.Count;
                var rating = count > 0 ? reviews.Average(r => r.Rating) : 0;
                
                _logger.LogInformation("Product {ProductId} has rating {Rating} from {Count} reviews", productId, rating, count);
                return (rating, count);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error getting rating for product {ProductId}", productId);
                throw;
            }
        }
    }
}
