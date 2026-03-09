using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;

namespace RealWorld.Application.Services;

public interface IArticleReadService
{
    Task<ArticleData?> FindByIdAsync(string id);
    Task<ArticleData?> FindBySlugAsync(string slug);
    Task<List<string>> QueryArticlesAsync(string? tag, string? author, string? favoritedBy, Page page);
    Task<int> CountArticleAsync(string? tag, string? author, string? favoritedBy);
    Task<List<ArticleData>> FindArticlesAsync(List<string> articleIds);
    Task<List<ArticleData>> FindArticlesOfAuthorsAsync(List<string> authors, Page page);
    Task<List<ArticleData>> FindArticlesOfAuthorsWithCursorAsync(List<string> authors, CursorPageParameter<DateTimeOffset> page);
    Task<int> CountFeedSizeAsync(List<string> authors);
    Task<List<string>> FindArticlesWithCursorAsync(string? tag, string? author, string? favoritedBy, CursorPageParameter<DateTimeOffset> page);
}
