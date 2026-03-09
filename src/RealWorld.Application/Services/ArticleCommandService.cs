using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Application.Services;

public class ArticleCommandService
{
    private readonly IArticleRepository _articleRepository;

    public ArticleCommandService(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Article> CreateArticleAsync(string title, string description, string body, List<string> tagList, User creator)
    {
        var article = new Article(title, description, body, tagList, creator.Id);
        await _articleRepository.SaveAsync(article);
        return article;
    }

    public async Task<Article> UpdateArticleAsync(Article article, string? title, string? description, string? body)
    {
        article.Update(title, description, body);
        await _articleRepository.SaveAsync(article);
        return article;
    }
}
