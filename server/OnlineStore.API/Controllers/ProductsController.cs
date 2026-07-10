using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using OnlineStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Prometheus;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;
        private readonly string _instanceId;
        
        // Создаём гистограмму с меткой "route"
        private static readonly Histogram ResponseSizeHistogram = Metrics
            .CreateHistogram("http_response_size_bytes", "Response size in bytes",
                new HistogramConfiguration
                {
                    Buckets = new[] { 100.0, 500.0, 1000.0, 5000.0, 10000.0, 50000.0, 100000.0 },
                    LabelNames = new[] { "route" }
                });

        public ProductsController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<ProductListItemDto>>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice,
            [FromQuery] bool? inStock,
            [FromQuery] string? sortBy,
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            var result = await _service.GetProductsAsync(search, minPrice, maxPrice, inStock, sortBy, descending, page, pageSize, cancellationToken);
            
            var json = JsonSerializer.Serialize(result);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            
            // Наблюдаем с меткой "route"
            ResponseSizeHistogram.WithLabels("api/products").Observe(sizeInBytes);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _service.GetProductAsync(id, cancellationToken);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(
            [FromBody] ProductCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _service.CreateProductAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(
            int id,
            [FromBody] ProductUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _service.UpdateProductAsync(id, dto, cancellationToken);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.DeleteProductAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}