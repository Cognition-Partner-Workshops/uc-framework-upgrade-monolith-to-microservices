using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface IArticleFavoriteRepository
{
    Task SaveAsync(ArticleFavorite articleFavorite);
    Task<ArticleFavorite?> FindAsync(string articleId, string userId);
    Task RemoveAsync(ArticleFavorite favorite);
}
