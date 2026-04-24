using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Services.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Prometheus;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly string _instanceId;
        
        private static readonly Histogram ResponseSizeHistogram = Metrics
            .CreateHistogram("http_response_size_bytes", "Response size in bytes",
                new HistogramConfiguration
                {
                    Buckets = new double[] { 100, 500, 1000, 5000, 10000, 50000, 100000 },
                    LabelNames = new[] { "route" }
                });

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteListItemDto>>> GetFavorites([FromQuery] int userId, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid();
                }
                
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId, cancellationToken);
                
                var json = JsonSerializer.Serialize(favorites);
                var sizeInBytes = Encoding.UTF8.GetByteCount(json);
                ResponseSizeHistogram.WithLabels("api/favorites").Observe(sizeInBytes);
                
                return Ok(favorites);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Favorites for user ID {userId} not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FavoriteResponseDto>> AddFavorite([FromBody] FavoriteCreateDto dto, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                dto.UserId = currentUserId;
                
                var favorite = await _favoriteService.AddFavoriteAsync(dto, cancellationToken);
                
                var json = JsonSerializer.Serialize(favorite);
                var sizeInBytes = Encoding.UTF8.GetByteCount(json);
                ResponseSizeHistogram.WithLabels("api/favorites").Observe(sizeInBytes);
                
                return favorite != null ? Ok(favorite) : NotFound("Product not found or already in favorites");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveFavorite([FromQuery] int userId, [FromQuery] int productId, CancellationToken cancellationToken = default)
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid();
                }
                
                var removed = await _favoriteService.RemoveFavoriteAsync(userId, productId, cancellationToken);
                
                var response = new { Message = removed ? "Product removed from favorites" : "Favorite not found" };
                var json = JsonSerializer.Serialize(response);
                var sizeInBytes = Encoding.UTF8.GetByteCount(json);
                ResponseSizeHistogram.WithLabels("api/favorites").Observe(sizeInBytes);
                
                return removed ? Ok(response) : NotFound(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}