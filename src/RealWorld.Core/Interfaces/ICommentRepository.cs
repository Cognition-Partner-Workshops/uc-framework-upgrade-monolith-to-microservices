using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface ICommentRepository
{
    Task SaveAsync(Comment comment);
    Task<Comment?> FindByIdAsync(string articleId, string id);
    Task RemoveAsync(Comment comment);
}
