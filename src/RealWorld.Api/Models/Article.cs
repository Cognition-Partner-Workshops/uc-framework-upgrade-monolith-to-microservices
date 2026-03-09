using System.Text.RegularExpressions;

namespace RealWorld.Api.Models;

public class Article
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? Author { get; set; }
    public List<ArticleTag> ArticleTags { get; set; } = new();
    public List<ArticleFavorite> Favorites { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    public void Update(string? title, string? description, string? body)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
            Slug = ToSlug(title);
            UpdatedAt = DateTime.UtcNow;
        }
        if (!string.IsNullOrEmpty(description))
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        if (!string.IsNullOrEmpty(body))
        {
            Body = body;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public static string ToSlug(string title)
    {
        return Regex.Replace(title.ToLowerInvariant(), @"[\&|\uFE30-\uFFA0|\'|\""\s\?\,\.]+", "-");
    }
}
