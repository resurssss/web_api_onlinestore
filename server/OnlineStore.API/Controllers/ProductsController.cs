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
        
        private static readonly Histogram ResponseSizeHistogram = Metrics
        .CreateHistogram("http_response_size_bytes", "Response size in bytes",
        new HistogramConfiguration
        {
            Buckets = new double[] { 100, 500, 1000, 5000, 10000, 50000, 100000 }
        });

        public ProductsController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        // GET: /api/products
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
            ResponseSizeHistogram.Observe(sizeInBytes);
            
            return Ok(result);
        }

        [HttpGet("raw-error")]
        [AllowAnonymous]
        public void RawError()
        {
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.ContentType = "text/plain";
            HttpContext.Response.WriteAsync("Internal Server Error");
        }

        [HttpGet("env")]
        [AllowAnonymous]
        public IActionResult GetEnv()
        {
            throw new Exception("Test 500 error");
            var instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID");
            return Ok(new { 
                instance_id = instanceId,
                container_ip = Request.HttpContext.Connection.LocalIpAddress?.ToString()
            });
        }
        
        // GET: /api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            try
            {
                var product = await _service.GetProductWithImagesAsync(id, cancellationToken);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {id} not found");
            }
        }

        // POST: /api/products/bulk-create
        [HttpPost("bulk-create")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<List<BulkOperationResultDto<ProductResponseDto>>>> BulkCreate([FromBody] List<ProductCreateDto> dtos, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            var result = await _service.BulkCreateAsync(dtos, cancellationToken);
            return Ok(result);
        }

        // PUT: /api/products/bulk-update
        [HttpPut("bulk-update")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<List<BulkOperationResultDto<ProductResponseDto>>>> BulkUpdate([FromBody] List<(int Id, ProductUpdateDto Dto)> items, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            var result = await _service.BulkUpdateAsync(items, cancellationToken);
            return Ok(result);
        }

        // DELETE: /api/products/bulk-delete
        [HttpDelete("bulk-delete")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<List<BulkOperationResultDto<object>>>> BulkDelete([FromBody] List<int> ids, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            var result = await _service.BulkDeleteAsync(ids, cancellationToken);
            return Ok(result);
        }
    }
}