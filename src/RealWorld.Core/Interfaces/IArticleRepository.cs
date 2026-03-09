using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface IArticleRepository
{
    Task SaveAsync(Article article);
    Task<Article?> FindByIdAsync(string id);
    Task<Article?> FindBySlugAsync(string slug);
    Task RemoveAsync(Article article);
}
