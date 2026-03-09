using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;

namespace RealWorld.Api.Controllers;

[ApiController]
[Route("articles/{slug}/favorite")]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    private User? GetCurrentUser() => HttpContext.Items["User"] as User;

    private async Task<ArticleDataDto> MapToArticleDto(Article article, User? currentUser)
    {
        var tagList = await _context.ArticleTags
            .Where(at => at.ArticleId == article.Id)
            .Include(at => at.Tag)
            .Select(at => at.Tag!.Name)
            .ToListAsync();

        var author = await _context.Users.FindAsync(article.UserId);
        var favoritesCount = await _context.ArticleFavorites.CountAsync(f => f.ArticleId == article.Id);
        var favorited = currentUser != null &&
            await _context.ArticleFavorites.AnyAsync(f => f.ArticleId == article.Id && f.UserId == currentUser.Id);
        var following = currentUser != null && author != null &&
            await _context.Follows.AnyAsync(f => f.UserId == currentUser.Id && f.TargetId == author.Id);

        return new ArticleDataDto
        {
            Slug = article.Slug,
            Title = article.Title,
            Description = article.Description,
            Body = article.Body,
            Favorited = favorited,
            FavoritesCount = favoritesCount,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            TagList = tagList,
            Author = new ProfileDataDto
            {
                Username = author?.Username ?? "",
                Bio = author?.Bio,
                Image = author?.Image,
                Following = following
            }
        };
    }

    [HttpPost]
    public async Task<IActionResult> FavoriteArticle(string slug)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var exists = await _context.ArticleFavorites.AnyAsync(f => f.ArticleId == article.Id && f.UserId == user.Id);
        if (!exists)
        {
            _context.ArticleFavorites.Add(new ArticleFavorite { ArticleId = article.Id, UserId = user.Id });
            await _context.SaveChangesAsync();
        }

        var articleDto = await MapToArticleDto(article, user);
        return Ok(new SingleArticleResponse { Article = articleDto });
    }

    [HttpDelete]
    public async Task<IActionResult> UnfavoriteArticle(string slug)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var favorite = await _context.ArticleFavorites.FirstOrDefaultAsync(f => f.ArticleId == article.Id && f.UserId == user.Id);
        if (favorite != null)
        {
            _context.ArticleFavorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        var articleDto = await MapToArticleDto(article, user);
        return Ok(new SingleArticleResponse { Article = articleDto });
    }
}
