using System.ComponentModel.DataAnnotations;

namespace RealWorld.Api.DTOs;

// --- Request DTOs ---

public class NewArticleRequest
{
    public NewArticleDto Article { get; set; } = new();
}

public class NewArticleDto
{
    [Required(ErrorMessage = "can't be empty")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "can't be empty")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "can't be empty")]
    public string Body { get; set; } = string.Empty;

    public List<string>? TagList { get; set; }
}

public class UpdateArticleRequest
{
    public UpdateArticleDto Article { get; set; } = new();
}

public class UpdateArticleDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Body { get; set; }
}

// --- Response DTOs ---

public class SingleArticleResponse
{
    public ArticleDataDto Article { get; set; } = new();
}

public class MultipleArticlesResponse
{
    public List<ArticleDataDto> Articles { get; set; } = new();
    public int ArticlesCount { get; set; }
}

public class ArticleDataDto
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool Favorited { get; set; }
    public int FavoritesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> TagList { get; set; } = new();
    public ProfileDataDto Author { get; set; } = new();
}
