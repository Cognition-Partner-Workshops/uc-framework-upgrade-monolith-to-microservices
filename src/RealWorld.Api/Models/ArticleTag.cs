namespace RealWorld.Api.Models;

public class ArticleTag
{
    public string ArticleId { get; set; } = string.Empty;
    public Article? Article { get; set; }

    public string TagId { get; set; } = string.Empty;
    public Tag? Tag { get; set; }
}
