using Microsoft.EntityFrameworkCore;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;
using RealWorld.Infrastructure.Data;

namespace RealWorld.Infrastructure.Repositories;

public class EfArticleRepository : IArticleRepository
{
    private readonly AppDbContext _context;

    public EfArticleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(Article article)
    {
        var existing = await _context.Articles.FindAsync(article.Id);
        if (existing == null)
        {
            await CreateNewAsync(article);
        }
        else
        {
            existing.Title = article.Title;
            existing.Slug = article.Slug;
            existing.Description = article.Description;
            existing.Body = article.Body;
            existing.UpdatedAt = article.UpdatedAt;
            await _context.SaveChangesAsync();
        }
    }

    private async Task CreateNewAsync(Article article)
    {
        foreach (var tag in article.Tags)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
            var targetTag = existingTag ?? tag;

            if (existingTag == null)
            {
                _context.Tags.Add(tag);
            }

            _context.Set<ArticleTag>().Add(new ArticleTag
            {
                ArticleId = article.Id,
                TagId = targetTag.Id
            });
        }

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
    }

    public async Task<Article?> FindByIdAsync(string id)
    {
        return await _context.Articles.FindAsync(id);
    }

    public async Task<Article?> FindBySlugAsync(string slug)
    {
        return await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
    }

    public async Task RemoveAsync(Article article)
    {
        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
    }
}
