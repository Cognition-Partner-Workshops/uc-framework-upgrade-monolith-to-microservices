using System.Text.RegularExpressions;

namespace RealWorld.Core.Entities;

public class Article
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<Tag> Tags { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Article() { }

    public Article(string title, string description, string body, List<string> tagList, string userId)
        : this(title, description, body, tagList, userId, DateTimeOffset.UtcNow)
    {
    }

    public Article(string title, string description, string body, List<string> tagList, string userId, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid().ToString();
        Slug = ToSlug(title);
        Title = title;
        Description = description;
        Body = body;
        Tags = tagList.Distinct().Select(t => new Tag(t)).ToList();
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public void Update(string? title, string? description, string? body)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
            Slug = ToSlug(title);
            UpdatedAt = DateTimeOffset.UtcNow;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
        if (!string.IsNullOrWhiteSpace(body))
        {
            Body = body;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    public static string ToSlug(string title)
    {
        return Regex.Replace(title.ToLowerInvariant(), @"[&\uFE30-\uFFA0'\""\s?,\.]+", "-");
    }

    public override bool Equals(object? obj) => obj is Article a && a.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
