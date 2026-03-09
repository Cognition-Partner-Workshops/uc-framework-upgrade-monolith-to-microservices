using Microsoft.EntityFrameworkCore;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;
using RealWorld.Infrastructure.Data;

namespace RealWorld.Infrastructure.Repositories;

public class EfArticleFavoriteRepository : IArticleFavoriteRepository
{
    private readonly AppDbContext _context;

    public EfArticleFavoriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(ArticleFavorite articleFavorite)
    {
        var existing = await FindAsync(articleFavorite.ArticleId, articleFavorite.UserId);
        if (existing == null)
        {
            _context.ArticleFavorites.Add(articleFavorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ArticleFavorite?> FindAsync(string articleId, string userId)
    {
        return await _context.ArticleFavorites
            .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId);
    }

    public async Task RemoveAsync(ArticleFavorite favorite)
    {
        _context.ArticleFavorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }
}
