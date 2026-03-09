using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;

namespace RealWorld.Api.Controllers;

[ApiController]
[Route("articles/{slug}/comments")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    private User? GetCurrentUser() => HttpContext.Items["User"] as User;

    private async Task<CommentDataDto> MapToCommentDto(Comment comment, User? currentUser)
    {
        var author = await _context.Users.FindAsync(comment.UserId);
        var following = currentUser != null && author != null &&
            await _context.Follows.AnyAsync(f => f.UserId == currentUser.Id && f.TargetId == author.Id);

        return new CommentDataDto
        {
            Id = comment.Id,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
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
    public async Task<IActionResult> CreateComment(string slug, [FromBody] NewCommentRequest request)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var comment = new Comment
        {
            Body = request.Comment.Body,
            UserId = user.Id,
            ArticleId = article.Id
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var commentDto = await MapToCommentDto(comment, user);
        return StatusCode(201, new SingleCommentResponse { Comment = commentDto });
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(string slug)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var user = GetCurrentUser();
        var comments = await _context.Comments
            .Where(c => c.ArticleId == article.Id)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var commentDtos = new List<CommentDataDto>();
        foreach (var comment in comments)
        {
            commentDtos.Add(await MapToCommentDto(comment, user));
        }

        return Ok(new MultipleCommentsResponse { Comments = commentDtos });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(string slug, string id)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if (article == null) return NotFound();

        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.ArticleId == article.Id);
        if (comment == null) return NotFound();

        if (comment.UserId != user.Id && article.UserId != user.Id)
            return StatusCode(403);

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
