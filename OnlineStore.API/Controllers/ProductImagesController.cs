using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OnlineStore.Services.Services;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductImagesController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /api/products/{productId}/images
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductImageResponseDto>>> GetProductImages(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var images = await _productService.GetProductImagesAsync(productId, cancellationToken);
                return Ok(images);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {productId} not found");
            }
        }

        // POST: /api/products/{productId}/images
        /// <summary>
        /// Загрузка изображения для продукта
        /// </summary>
        /// <param name="productId">ID продукта</param>
        /// <param name="fileService">Сервис работы с файлами</param>
        /// <param name="file">Файл изображения</param>
        /// <param name="fileId">ID существующего файла</param>
        /// <param name="isMain">Признак основного изображения</param>
        /// <param name="order">Порядок отображения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Добавленное изображение продукта</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductImageResponseDto>> UploadProductImage(
            int productId,
            [FromServices] FileService fileService,
            [FromForm] IFormFile? file = null,
            [FromForm] int? fileId = null,
            [FromForm] bool isMain = false,
            [FromForm] int order = 0,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка существования продукта
                var product = await _productService.GetProductAsync(productId, cancellationToken);
                
                // Проверка прав доступа (владелец или админ)
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Невозможно получить ID пользователя");
                }
                
                // В данном случае разрешаем загрузку изображений только администраторам
                // В реальном приложении можно добавить проверку владельца продукта
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("Администратор");
                if (!isAdmin)
                {
                    return Forbid();
                }
                
                // Если предоставлен файл, загружаем его
                if (file != null)
                {
                    var fileMetadata = await fileService.UploadFileAsync(file, userId, true, null);
                    fileId = fileMetadata.Id;
                }
                else if (!fileId.HasValue)
                {
                    return BadRequest("Необходимо предоставить файл или FileId");
                }
                
                // Создаем DTO для передачи в сервис
                var dto = new ProductImageCreateDto
                {
                    FileId = fileId,
                    IsMain = isMain,
                    Order = order
                };
                
                var image = await _productService.AddProductImageAsync(productId, dto, cancellationToken);
                return CreatedAtAction(nameof(GetProductImages), new { productId = productId }, image);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {productId} not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // DELETE: /api/products/{productId}/images/{imageId}
        [HttpDelete("{imageId}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteProductImage(int productId, int imageId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _productService.RemoveProductImageAsync(productId, imageId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) when (ex.Message.Contains("Product"))
            {
                return NotFound($"Product with ID {productId} not found");
            }
            catch (KeyNotFoundException ex) when (ex.Message.Contains("Image"))
            {
                return NotFound($"Image with ID {imageId} not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}