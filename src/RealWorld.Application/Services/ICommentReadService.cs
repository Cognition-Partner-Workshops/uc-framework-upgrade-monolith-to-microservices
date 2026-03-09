using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;

namespace RealWorld.Application.Services;

public interface ICommentReadService
{
    Task<CommentData?> FindByIdAsync(string id);
    Task<List<CommentData>> FindByArticleIdAsync(string articleId);
    Task<List<CommentData>> FindByArticleIdWithCursorAsync(string articleId, CursorPageParameter<DateTimeOffset> page);
}
