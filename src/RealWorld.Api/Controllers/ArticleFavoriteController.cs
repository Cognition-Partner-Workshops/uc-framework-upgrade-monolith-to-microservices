using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Api.Exceptions;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
[Authorize]
public class ArticleFavoriteController : ControllerBase
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleFavoriteRepository _articleFavoriteRepository;
    private readonly ArticleQueryService _articleQueryService;

    public ArticleFavoriteController(
        IArticleRepository articleRepository,
        IArticleFavoriteRepository articleFavoriteRepository,
        ArticleQueryService articleQueryService)
    {
        _articleRepository = articleRepository;
        _articleFavoriteRepository = articleFavoriteRepository;
        _articleQueryService = articleQueryService;
    }

    [HttpPost("/articles/{slug}/favorite")]
    public async Task<IActionResult> FavoriteArticle(string slug)
    {
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        var user = GetUser();
        var favorite = new ArticleFavorite(article.Id, user.Id);
        await _articleFavoriteRepository.SaveAsync(favorite);

        var articleData = await _articleQueryService.FindByIdAsync(article.Id, user);
        return Ok(new { article = articleData });
    }

    [HttpDelete("/articles/{slug}/favorite")]
    public async Task<IActionResult> UnfavoriteArticle(string slug)
    {
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        var user = GetUser();
        var favorite = await _articleFavoriteRepository.FindAsync(article.Id, user.Id);
        if (favorite != null)
            await _articleFavoriteRepository.RemoveAsync(favorite);

        var articleData = await _articleQueryService.FindByIdAsync(article.Id, user);
        return Ok(new { article = articleData });
    }

    private User GetUser()
    {
        return HttpContext.Items["User"] as User
            ?? throw new UnauthorizedAccessException("Authentication required");
    }
}
