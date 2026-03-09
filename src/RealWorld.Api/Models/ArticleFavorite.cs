namespace RealWorld.Api.Models;

public class ArticleFavorite
{
    public string ArticleId { get; set; } = string.Empty;
    public Article? Article { get; set; }

    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}
