using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using OnlineStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;

        public ProductsController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: /api/products
        [HttpGet]
        [AllowAnonymous] // Разрешаем анонимный доступ к списку продуктов
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
            var result = await _service.GetProductsAsync(search, minPrice, maxPrice, inStock, sortBy, descending, page, pageSize, cancellationToken);
            return Ok(result);
        }
        
        // GET: /api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
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
        [Authorize(Roles = "Администратор")] // Только администраторы могут создавать продукты
        public async Task<ActionResult<List<BulkOperationResultDto<ProductResponseDto>>>> BulkCreate([FromBody] List<ProductCreateDto> dtos, CancellationToken cancellationToken = default)
        {
            var result = await _service.BulkCreateAsync(dtos, cancellationToken);
            return Ok(result);
        }

        // PUT: /api/products/bulk-update
        [HttpPut("bulk-update")]
        [Authorize(Roles = "Администратор")] // Только администраторы могут обновлять продукты
        public async Task<ActionResult<List<BulkOperationResultDto<ProductResponseDto>>>> BulkUpdate([FromBody] List<(int Id, ProductUpdateDto Dto)> items, CancellationToken cancellationToken = default)
        {
            var result = await _service.BulkUpdateAsync(items, cancellationToken);
            return Ok(result);
        }

        // DELETE: /api/products/bulk-delete
        [HttpDelete("bulk-delete")]
        [Authorize(Roles = "Администратор")] // Только администраторы могут удалять продукты
        public async Task<ActionResult<List<BulkOperationResultDto<object>>>> BulkDelete([FromBody] List<int> ids, CancellationToken cancellationToken = default)
        {
            var result = await _service.BulkDeleteAsync(ids, cancellationToken);
            return Ok(result);
        }
    }
}