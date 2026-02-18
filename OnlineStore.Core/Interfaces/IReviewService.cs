using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponseDto>> GetReviewsAsync(int? productId = null, CancellationToken cancellationToken = default);
        Task<PagedResultDto<ReviewResponseDto>> GetReviewsAsync(int productId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<ReviewResponseDto> AddReviewAsync(ReviewCreateDto dto, CancellationToken cancellationToken = default);
        Task<(double rating, int count)> GetProductRatingAsync(int productId, CancellationToken cancellationToken = default);
    }
}