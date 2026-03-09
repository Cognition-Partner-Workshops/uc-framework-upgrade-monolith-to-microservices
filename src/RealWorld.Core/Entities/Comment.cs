namespace RealWorld.Core.Entities;

public class Comment
{
    public string Id { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ArticleId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public Comment() { }

    public Comment(string body, string userId, string articleId)
    {
        Id = Guid.NewGuid().ToString();
        Body = body;
        UserId = userId;
        ArticleId = articleId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public override bool Equals(object? obj) => obj is Comment c && c.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
