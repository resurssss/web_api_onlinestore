using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteListItemDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<FavoriteListItemDto>> GetUserFavoritesAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<FavoriteResponseDto> AddFavoriteAsync(FavoriteCreateDto dto, CancellationToken cancellationToken = default);
        Task<bool> RemoveFavoriteAsync(int userId, int productId, CancellationToken cancellationToken = default);
    }
}