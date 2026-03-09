namespace RealWorld.Api.Models;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Body { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ArticleId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? Author { get; set; }
    public Article? Article { get; set; }
}
