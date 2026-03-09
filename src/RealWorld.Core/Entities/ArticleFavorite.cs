namespace RealWorld.Core.Entities;

public class ArticleFavorite
{
    public string ArticleId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public ArticleFavorite() { }

    public ArticleFavorite(string articleId, string userId)
    {
        ArticleId = articleId;
        UserId = userId;
    }
}
