using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Получить список отзывов, можно фильтровать по продукту
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Разрешаем анонимный доступ к списку отзывов
        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviews([FromQuery] int? productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsAsync(productId, cancellationToken);
                return Ok(reviews);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Reviews for product ID {productId} not found");
            }
        }

        /// <summary>
        /// Добавить отзыв
        /// </summary>
        [HttpPost]
        [Authorize] // Только авторизованные пользователи могут добавлять отзывы
        public async Task<ActionResult<ReviewResponseDto>> AddReview([FromBody] ReviewCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем ID текущего пользователя из Claims и устанавливаем его в DTO
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                dto.UserId = currentUserId; // Нельзя подменить через DTO
                
                var review = await _reviewService.AddReviewAsync(dto, cancellationToken);
                return review != null ? Ok(review) : NotFound("Product not found");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {dto.ProductId} not found");
            }
        }

        /// <summary>
        /// Получить рейтинг и количество отзывов для продукта
        /// </summary>
        [HttpGet("rating")]
        [AllowAnonymous] // Разрешаем анонимный доступ к рейтингу
        public async Task<ActionResult> GetProductRating([FromQuery] int productId, CancellationToken cancellationToken = default)
        {
            var (rating, count) = await _reviewService.GetProductRatingAsync(productId, cancellationToken);
            return Ok(new { Rating = rating, ReviewCount = count });
        }
    }
}
