using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Services.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Требуется авторизация для всех endpoints
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Получить список избранного пользователя
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteListItemDto>>> GetFavorites([FromQuery] int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка прав доступа: пользователь может получить только свое избранное
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Проверка, что запрашивается избранное текущего пользователя или пользователь - админ/модератор
                if (userId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid(); // Запрет доступа к чужому избранному
                }
                
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId, cancellationToken);
                return Ok(favorites);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Favorites for user ID {userId} not found");
            }
        }

        /// <summary>
        /// Добавить продукт в избранное
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FavoriteResponseDto>> AddFavorite([FromBody] FavoriteCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем ID текущего пользователя из Claims и устанавливаем его в DTO
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                dto.UserId = currentUserId; // Нельзя подменить через DTO
                
                var favorite = await _favoriteService.AddFavoriteAsync(dto, cancellationToken);
                return favorite != null ? Ok(favorite) : NotFound("Product not found or already in favorites");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Удалить продукт из избранного
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> RemoveFavorite([FromQuery] int userId, [FromQuery] int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка прав доступа: пользователь может удалять только из своего избранного
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Проверка, что удаляется из избранного текущего пользователя или пользователь - админ/модератор
                if (userId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid(); // Запрет доступа к чужому избранному
                }
                
                var removed = await _favoriteService.RemoveFavoriteAsync(userId, productId, cancellationToken);
                return removed ? Ok(new { Message = "Product removed from favorites" }) : NotFound("Favorite not found");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
