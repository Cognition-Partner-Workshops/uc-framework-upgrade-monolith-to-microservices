using Microsoft.EntityFrameworkCore;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;
using RealWorld.Infrastructure.Data;

namespace RealWorld.Infrastructure.Repositories;

public class EfCommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public EfCommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<Comment?> FindByIdAsync(string articleId, string id)
    {
        return await _context.Comments
            .FirstOrDefaultAsync(c => c.ArticleId == articleId && c.Id == id);
    }

    public async Task RemoveAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
