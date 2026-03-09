using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;

namespace RealWorld.Api.Controllers;

[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArticlesController(AppDbContext context)
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

    [HttpPost("/articles")]
    public async Task<IActionResult> CreateArticle([FromBody] NewArticleRequest request)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var dto = request.Article;

        if (await _context.Articles.AnyAsync(a => a.Slug == Article.ToSlug(dto.Title)))
            return UnprocessableEntity(new { errors = new { title = new[] { "has already been taken" } } });

        var article = new Article
        {
            UserId = user.Id,
            Slug = Article.ToSlug(dto.Title),
            Title = dto.Title,
            Description = dto.Description,
            Body = dto.Body
        };

        _context.Articles.Add(article);

        if (dto.TagList != null)
        {
            foreach (var tagName in dto.TagList.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                }
                _context.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
            }
        }

        await _context.SaveChangesAsync();

        var articleDto = await MapToArticleDto(article, user);
        return Ok(new SingleArticleResponse { Article = articleDto });
    }

    [HttpGet("/articles")]
    public async Task<IActionResult> GetArticles(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20,
        [FromQuery] string? tag = null,
        [FromQuery] string? author = null,
        [FromQuery] string? favorited = null)
    {
        var user = GetCurrentUser();
        limit = Math.Min(limit, 100);
        if (offset < 0) offset = 0;

        var query = _context.Articles.AsQueryable();

        if (!string.IsNullOrEmpty(tag))
        {
            var tagEntity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag);
            if (tagEntity != null)
            {
                var articleIds = _context.ArticleTags.Where(at => at.TagId == tagEntity.Id).Select(at => at.ArticleId);
                query = query.Where(a => articleIds.Contains(a.Id));
            }
            else
            {
                return Ok(new MultipleArticlesResponse { Articles = new(), ArticlesCount = 0 });
            }
        }

        if (!string.IsNullOrEmpty(author))
        {
            var authorUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == author);
            if (authorUser != null)
            {
                query = query.Where(a => a.UserId == authorUser.Id);
            }
            else
            {
                return Ok(new MultipleArticlesResponse { Articles = new(), ArticlesCount = 0 });
            }
        }

        if (!string.IsNullOrEmpty(favorited))
        {
            var favUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == favorited);
            if (favUser != null)
            {
                var favArticleIds = _context.ArticleFavorites.Where(f => f.UserId == favUser.Id).Select(f => f.ArticleId);
                query = query.Where(a => favArticleIds.Contains(a.Id));
            }
            else
            {
                return Ok(new MultipleArticlesResponse { Articles = new(), ArticlesCount = 0 });
            }
        }

        var totalCount = await query.CountAsync();
        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var articleDtos = new List<ArticleDataDto>();
        foreach (var article in articles)
        {
            articleDtos.Add(await MapToArticleDto(article, user));
        }

        return Ok(new MultipleArticlesResponse { Articles = articleDtos, ArticlesCount = totalCount });
    }

    [HttpGet("/articles/feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        limit = Math.Min(limit, 100);
        if (offset < 0) offset = 0;

        var followedUserIds = await _context.Follows
            .Where(f => f.UserId == user.Id)
            .Select(f => f.TargetId)
            .ToListAsync();

        if (followedUserIds.Count == 0)
            return Ok(new MultipleArticlesResponse { Articles = new(), ArticlesCount = 0 });

        var query = _context.Articles.Where(a => followedUserIds.Contains(a.UserId));
        var totalCount = await query.CountAsync();

        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var articleDtos = new List<ArticleDataDto>();
        foreach (var article in articles)
        {
            articleDtos.Add(await MapToArticleDto(article, user));
        }

        return Ok(new MultipleArticlesResponse { Articles = articleDtos, ArticlesCount = totalCount });
    }

    [HttpGet("/articles/{slug}")]
    public async Task<IActionResult> GetArticle(string slug)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var user = GetCurrentUser();
        var articleDto = await MapToArticleDto(article, user);
        return Ok(new SingleArticleResponse { Article = articleDto });
    }

    [HttpPut("/articles/{slug}")]
    public async Task<IActionResult> UpdateArticle(string slug, [FromBody] UpdateArticleRequest request)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        if (article.UserId != user.Id) return StatusCode(403);

        var dto = request.Article;
        article.Update(dto.Title, dto.Description, dto.Body);

        _context.Articles.Update(article);
        await _context.SaveChangesAsync();

        var articleDto = await MapToArticleDto(article, user);
        return Ok(new SingleArticleResponse { Article = articleDto });
    }

    [HttpDelete("/articles/{slug}")]
    public async Task<IActionResult> DeleteArticle(string slug)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        if (article.UserId != user.Id) return StatusCode(403);

        // Remove related data
        var articleTags = _context.ArticleTags.Where(at => at.ArticleId == article.Id);
        _context.ArticleTags.RemoveRange(articleTags);

        var favorites = _context.ArticleFavorites.Where(f => f.ArticleId == article.Id);
        _context.ArticleFavorites.RemoveRange(favorites);

        var comments = _context.Comments.Where(c => c.ArticleId == article.Id);
        _context.Comments.RemoveRange(comments);

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
