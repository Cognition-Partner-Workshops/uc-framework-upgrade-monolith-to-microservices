using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface IAuthorizationService
{
    static bool CanWriteArticle(User user, Article article)
    {
        return user.Id == article.UserId;
    }

    static bool CanWriteComment(User user, Article article, Comment comment)
    {
        return user.Id == article.UserId || user.Id == comment.UserId;
    }
}
