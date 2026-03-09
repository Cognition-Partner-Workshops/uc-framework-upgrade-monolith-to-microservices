using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Application.Pagination;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly ArticleQueryService _articleQueryService;
    private readonly ArticleCommandService _articleCommandService;
    private readonly IArticleRepository _articleRepository;

    public ArticlesController(
        ArticleQueryService articleQueryService,
        ArticleCommandService articleCommandService,
        IArticleRepository articleRepository)
    {
        _articleQueryService = articleQueryService;
        _articleCommandService = articleCommandService;
        _articleRepository = articleRepository;
    }

    [HttpGet("/articles")]
    public async Task<IActionResult> GetArticles(
        [FromQuery] string? tag,
        [FromQuery] string? author,
        [FromQuery] string? favorited,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var user = GetOptionalUser();
        var page = new Page(offset, limit);
        var result = await _articleQueryService.FindRecentArticlesAsync(tag, author, favorited, page, user);
        return Ok(result);
    }

    [HttpGet("/articles/feed")]
    [Authorize]
    public async Task<IActionResult> GetFeed(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var user = GetUser();
        var page = new Page(offset, limit);
        var result = await _articleQueryService.FindUserFeedAsync(user, page);
        return Ok(result);
    }

    [HttpPost("/articles")]
    [Authorize]
    public async Task<IActionResult> CreateArticle([FromBody] NewArticleParam param)
    {
        var user = GetUser();
        var article = await _articleCommandService.CreateArticleAsync(
            param.Article.Title,
            param.Article.Description,
            param.Article.Body,
            param.Article.TagList ?? new List<string>(),
            user);

        var articleData = await _articleQueryService.FindByIdAsync(article.Id, user);
        return Ok(new { article = articleData });
    }

    private User? GetOptionalUser()
    {
        return HttpContext.Items["User"] as User;
    }

    private User GetUser()
    {
        return HttpContext.Items["User"] as User
            ?? throw new UnauthorizedAccessException("Authentication required");
    }

    public class NewArticleParam
    {
        [JsonPropertyName("article")]
        public NewArticleData Article { get; set; } = new();
    }

    public class NewArticleData
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string>? TagList { get; set; }
    }
}
