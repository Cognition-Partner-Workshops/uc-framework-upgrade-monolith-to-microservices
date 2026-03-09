using System.Text.Json.Serialization;

namespace RealWorld.Application.DTOs;

public class CommentData
{
    public string Id { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    [JsonIgnore]
    public string ArticleId { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("author")]
    public ProfileData ProfileData { get; set; } = new();
}
