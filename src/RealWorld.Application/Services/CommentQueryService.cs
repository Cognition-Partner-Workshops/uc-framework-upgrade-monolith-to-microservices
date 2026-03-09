using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;
using RealWorld.Core.Entities;

namespace RealWorld.Application.Services;

public class CommentQueryService
{
    private readonly ICommentReadService _commentReadService;
    private readonly IUserRelationshipQueryService _userRelationshipQueryService;

    public CommentQueryService(
        ICommentReadService commentReadService,
        IUserRelationshipQueryService userRelationshipQueryService)
    {
        _commentReadService = commentReadService;
        _userRelationshipQueryService = userRelationshipQueryService;
    }

    public async Task<CommentData?> FindByIdAsync(string id, User user)
    {
        var commentData = await _commentReadService.FindByIdAsync(id);
        if (commentData == null)
            return null;

        commentData.ProfileData.Following = await _userRelationshipQueryService
            .IsUserFollowingAsync(user.Id, commentData.ProfileData.Id);

        return commentData;
    }

    public async Task<List<CommentData>> FindByArticleIdAsync(string articleId, User? user)
    {
        var comments = await _commentReadService.FindByArticleIdAsync(articleId);
        if (comments.Count > 0 && user != null)
        {
            var authorIds = comments.Select(c => c.ProfileData.Id).ToList();
            var followingAuthors = await _userRelationshipQueryService
                .FollowingAuthorsAsync(user.Id, authorIds);
            foreach (var comment in comments)
            {
                if (followingAuthors.Contains(comment.ProfileData.Id))
                    comment.ProfileData.Following = true;
            }
        }
        return comments;
    }

    public async Task<CursorPager<CommentDataNode>> FindByArticleIdWithCursorAsync(
        string articleId, User? user, CursorPageParameter<DateTimeOffset> page)
    {
        var comments = await _commentReadService.FindByArticleIdWithCursorAsync(articleId, page);
        if (comments.Count == 0)
            return new CursorPager<CommentDataNode>(new List<CommentDataNode>(), page.Direction, false);

        if (user != null)
        {
            var authorIds = comments.Select(c => c.ProfileData.Id).ToList();
            var followingAuthors = await _userRelationshipQueryService
                .FollowingAuthorsAsync(user.Id, authorIds);
            foreach (var comment in comments)
            {
                if (followingAuthors.Contains(comment.ProfileData.Id))
                    comment.ProfileData.Following = true;
            }
        }

        bool hasExtra = comments.Count > page.Limit;
        if (hasExtra)
            comments.RemoveAt(page.Limit);

        if (!page.IsNext())
            comments.Reverse();

        return new CursorPager<CommentDataNode>(
            comments.Select(c => new CommentDataNode(c)).ToList(),
            page.Direction, hasExtra);
    }
}

public class CommentDataNode : INode
{
    public CommentData Data { get; }

    public CommentDataNode(CommentData data)
    {
        Data = data;
    }

    public DateTimeCursor GetCursor()
    {
        return new DateTimeCursor(Data.CreatedAt);
    }
}
