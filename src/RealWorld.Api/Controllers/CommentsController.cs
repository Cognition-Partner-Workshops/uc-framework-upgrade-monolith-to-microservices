using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Api.Exceptions;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly CommentQueryService _commentQueryService;

    public CommentsController(
        IArticleRepository articleRepository,
        ICommentRepository commentRepository,
        CommentQueryService commentQueryService)
    {
        _articleRepository = articleRepository;
        _commentRepository = commentRepository;
        _commentQueryService = commentQueryService;
    }

    [HttpGet("/articles/{slug}/comments")]
    public async Task<IActionResult> GetComments(string slug)
    {
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        var user = GetOptionalUser();
        var comments = await _commentQueryService.FindByArticleIdAsync(article.Id, user);
        return Ok(new { comments });
    }

    [HttpPost("/articles/{slug}/comments")]
    public async Task<IActionResult> CreateComment(string slug, [FromBody] NewCommentParam param)
    {
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        var user = GetUser();
        var comment = new Comment(param.Comment.Body, user.Id, article.Id);
        await _commentRepository.SaveAsync(comment);

        var commentData = await _commentQueryService.FindByIdAsync(comment.Id, user);
        return Ok(new { comment = commentData });
    }

    [HttpDelete("/articles/{slug}/comments/{id}")]
    public async Task<IActionResult> DeleteComment(string slug, string id)
    {
        var article = await _articleRepository.FindBySlugAsync(slug);
        if (article == null)
            throw new ResourceNotFoundException();

        var user = GetUser();
        var comment = await _commentRepository.FindByIdAsync(article.Id, id);
        if (comment == null)
            throw new ResourceNotFoundException();

        if (!RealWorld.Core.Interfaces.IAuthorizationService.CanWriteComment(user, article, comment))
            throw new NoAuthorizationException();

        await _commentRepository.RemoveAsync(comment);
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

    public class NewCommentParam
    {
        [JsonPropertyName("comment")]
        public NewCommentData Comment { get; set; } = new();
    }

    public class NewCommentData
    {
        public string Body { get; set; } = string.Empty;
    }
}
