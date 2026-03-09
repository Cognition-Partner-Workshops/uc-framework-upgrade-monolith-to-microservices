using RealWorld.Application.DTOs;
using RealWorld.Core.Entities;

namespace RealWorld.Application.Services;

public interface IArticleFavoritesReadService
{
    Task<bool> IsUserFavoriteAsync(string userId, string articleId);
    Task<int> ArticleFavoriteCountAsync(string articleId);
    Task<List<ArticleFavoriteCount>> ArticlesFavoriteCountAsync(List<string> ids);
    Task<HashSet<string>> UserFavoritesAsync(List<string> ids, User currentUser);
}
