using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Api.Exceptions;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class ArticleController : ControllerBase
{
    private readonly ArticleQueryService _articleQueryService;
    private readonly ArticleCommandService _articleCommandService;
    private readonly IArticleRepository _articleRepository;

    public ArticleController(
        ArticleQueryService articleQueryService,
        ArticleCommandService articleCommandService,
        IArticleRepository articleRepository)
    {
        _articleQueryService = articleQueryService;
        _articleCommandService = articleCommandService;
        _articleRepository = articleRepository;
    }

    [HttpGet("/articles/{slug}")]
    public async Task<IActionResult> GetArticle(string slug)
    {
        var user = GetOptionalUser();
        var articleData = await _articleQueryService.FindBySlugAsync(slug, user);
        if (articleData == null)
            throw new ResourceNotFoundException();
        return Ok(new { article = articleData });
    }

    [HttpPut("/articles/{slug}")]
    [Authorize]
    public async Task<IActionResult> UpdateArticle(string slug, [FromBody] UpdateArticleParam param)
    {
        var user = GetUser();
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        if (!RealWorld.Core.Interfaces.IAuthorizationService.CanWriteArticle(user, article))
            throw new NoAuthorizationException();

        await _articleCommandService.UpdateArticleAsync(article, param.Article.Title, param.Article.Description, param.Article.Body);

        var articleData = await _articleQueryService.FindByIdAsync(article.Id, user);
        return Ok(new { article = articleData });
    }

    [HttpDelete("/articles/{slug}")]
    [Authorize]
    public async Task<IActionResult> DeleteArticle(string slug)
    {
        var user = GetUser();
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        if (!RealWorld.Core.Interfaces.IAuthorizationService.CanWriteArticle(user, article))
            throw new NoAuthorizationException();

        await _articleRepository.RemoveAsync(article);
        return NoContent();
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

    public class UpdateArticleParam
    {
        [JsonPropertyName("article")]
        public UpdateArticleData Article { get; set; } = new();
    }

    public class UpdateArticleData
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Body { get; set; }
    }
}
