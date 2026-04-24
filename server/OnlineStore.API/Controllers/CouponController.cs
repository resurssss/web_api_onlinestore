using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Services.Services;
using OnlineStore.Core.Interfaces;
using System.Text;
using System.Text.Json;
using Prometheus;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly string _instanceId;
        
        private static readonly Histogram ResponseSizeHistogram = Metrics
            .CreateHistogram("http_response_size_bytes", "Response size in bytes",
                new HistogramConfiguration
                {
                    Buckets = new double[] { 100, 500, 1000, 5000, 10000, 50000, 100000 },
                    LabelNames = new[] { "route" }
                });

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponListItemDto>>> GetAll([FromQuery] string? code = null, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            var coupons = await _couponService.GetAllAsync(code, cancellationToken);
            
            var json = JsonSerializer.Serialize(coupons);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("api/coupons").Observe(sizeInBytes);
            
            return Ok(coupons);
        }

        [HttpGet("by-id")]
        public async Task<ActionResult<CouponResponseDto>> GetById([FromQuery] int id, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            var coupon = await _couponService.GetByIdAsync(id, cancellationToken);
            
            var json = JsonSerializer.Serialize(coupon);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("api/coupons/by-id").Observe(sizeInBytes);
            
            return coupon != null ? Ok(coupon) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<CouponResponseDto>> Create([FromBody] CouponCreateDto dto, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            var coupon = await _couponService.CreateAsync(dto, cancellationToken);
            
            var json = JsonSerializer.Serialize(coupon);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("api/coupons/create").Observe(sizeInBytes);
            
            return coupon != null ? CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon) : BadRequest();
        }

        [HttpPut]
        public async Task<ActionResult<CouponResponseDto>> Update([FromQuery] string id, [FromBody] CouponUpdateDto dto, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest("Invalid coupon ID format");
            }
            
            var coupon = await _couponService.UpdateAsync(intId, dto, cancellationToken);
            
            var json = JsonSerializer.Serialize(coupon);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("api/coupons/update").Observe(sizeInBytes);
            
            return coupon != null ? Ok(coupon) : NotFound();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string id, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest("Invalid coupon ID format");
            }
            
            var deleted = await _couponService.DeleteAsync(intId, cancellationToken);
            
            var response = new { deleted };
            var json = JsonSerializer.Serialize(response);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("api/coupons/delete").Observe(sizeInBytes);
            
            return deleted ? NoContent() : NotFound();
        }
    }
}