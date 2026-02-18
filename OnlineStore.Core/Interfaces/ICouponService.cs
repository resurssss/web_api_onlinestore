using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponListItemDto>> GetAllAsync(string? code = null, CancellationToken cancellationToken = default);
        Task<PagedResultDto<CouponListItemDto>> GetCouponsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<CouponResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CouponResponseDto?> CreateAsync(CouponCreateDto dto, CancellationToken cancellationToken = default);
        Task<CouponResponseDto?> UpdateAsync(int id, CouponUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}