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
    public class CouponService : ICouponService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponService> _logger;

        public CouponService(OnlineStoreDbContext context, IMapper mapper, ILogger<CouponService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public virtual async Task<IEnumerable<CouponListItemDto>> GetAllAsync(string? code = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all coupons with code filter: {Code}", code);
            
            var query = _context.Coupons.AsQueryable();
            
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(c => c.Code.ToLower().Contains(code.ToLower()));
            }
            
            var coupons = await query.ToListAsync(cancellationToken);
            
            _logger.LogInformation("Retrieved {Count} coupons", coupons.Count());
            return _mapper.Map<IEnumerable<CouponListItemDto>>(coupons);
        }
        
        public async Task<PagedResultDto<CouponListItemDto>> GetCouponsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged coupons - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var query = _context.Coupons.AsQueryable();
            var totalCount = await query.CountAsync(cancellationToken);
            var pagedCoupons = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            
            _logger.LogInformation("Retrieved {Count} coupons for page {Page}", pagedCoupons.Count, page);
            
            return new PagedResultDto<CouponListItemDto>
            {
                Items = _mapper.Map<IEnumerable<CouponListItemDto>>(pagedCoupons),
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<CouponResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting coupon by ID: {CouponId}", id);
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            
            if (coupon == null)
            {
                _logger.LogWarning("Coupon not found with ID: {CouponId}", id);
                return null;
            }
            
            _logger.LogInformation("Successfully retrieved coupon {CouponCode} (ID: {CouponId})", coupon.Code, id);
            return _mapper.Map<CouponResponseDto>(coupon);
        }

        public async Task<CouponResponseDto?> CreateAsync(CouponCreateDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new coupon with code: {CouponCode}", dto?.Code ?? "Unknown");
            
            try
            {
                // Проверяем уникальность кода купона
                if (dto?.Code != null)
                {
                    var existingCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToLower() == dto.Code.ToLower(), cancellationToken);
                    if (existingCoupon != null)
                    {
                        _logger.LogWarning("Coupon with code {CouponCode} already exists", dto.Code);
                        return null;
                    }
                }
                
                var coupon = _mapper.Map<Coupon>(dto);
                coupon.CreatedAt = DateTime.UtcNow;
                coupon.UpdatedAt = DateTime.UtcNow;
                
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Successfully created coupon {CouponCode} with ID: {CouponId}", coupon.Code, coupon.Id);
                return _mapper.Map<CouponResponseDto>(coupon);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error creating coupon with code: {CouponCode}", dto?.Code ?? "Unknown");
                throw;
            }
        }

        public async Task<CouponResponseDto?> UpdateAsync(int id, CouponUpdateDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating coupon with ID: {CouponId}", id);
            
            try
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
                
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon not found for update with ID: {CouponId}", id);
                    return null;
                }
                
                // Проверяем уникальность кода купона (если он изменился)
                if (dto?.Code != null && dto.Code != coupon.Code)
                {
                    var existingCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToLower() == dto.Code.ToLower() && c.Id != id, cancellationToken);
                    if (existingCoupon != null)
                    {
                        _logger.LogWarning("Coupon with code {CouponCode} already exists", dto.Code);
                        return null;
                    }
                }
                
                _mapper.Map(dto, coupon);
                coupon.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Successfully updated coupon {CouponCode} (ID: {CouponId})", coupon.Code, id);
                return _mapper.Map<CouponResponseDto>(coupon);
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error updating coupon with ID: {CouponId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting coupon with ID: {CouponId}", id);
            
            try
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
                
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon not found for deletion with ID: {CouponId}", id);
                    return false;
                }
                
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully deleted coupon with ID: {CouponId}", id);
                return true;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                _logger.LogError(ex, "Database error deleting coupon with ID: {CouponId}", id);
                throw new InvalidOperationException($"Failed to delete coupon with ID: {id}", ex);
            }
        }
    }
}
