using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Services.Services;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Получить все купоны
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponListItemDto>>> GetAll([FromQuery] string? code = null, CancellationToken cancellationToken = default)
        {
            var coupons = await _couponService.GetAllAsync(code, cancellationToken);
            return Ok(coupons);
        }

        /// <summary>
        /// Получить купон по Id
        /// </summary>
        [HttpGet("by-id")]
        public async Task<ActionResult<CouponResponseDto>> GetById([FromQuery] int id, CancellationToken cancellationToken = default)
        {
            var coupon = await _couponService.GetByIdAsync(id, cancellationToken);
            return coupon != null ? Ok(coupon) : NotFound();
        }

        /// <summary>
        /// Создать новый купон
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CouponResponseDto>> Create([FromBody] CouponCreateDto dto, CancellationToken cancellationToken = default)
        {
            var coupon = await _couponService.CreateAsync(dto, cancellationToken);
            return coupon != null ? CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon) : BadRequest();
        }

        /// <summary>
        /// Обновить купон
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<CouponResponseDto>> Update([FromQuery] string id, [FromBody] CouponUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest("Invalid coupon ID format");
            }
            
            var coupon = await _couponService.UpdateAsync(intId, dto, cancellationToken);
            return coupon != null ? Ok(coupon) : NotFound();
        }

        /// <summary>
        /// Удалить купон
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string id, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest("Invalid coupon ID format");
            }
            
            var deleted = await _couponService.DeleteAsync(intId, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
