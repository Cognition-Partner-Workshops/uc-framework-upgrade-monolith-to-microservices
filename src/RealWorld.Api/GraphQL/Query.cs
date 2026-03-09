using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;

namespace RealWorld.Api.GraphQL;

public class Query
{
    public async Task<ArticleData?> GetArticle(
        [Service] ArticleQueryService articleQueryService,
        string slug)
    {
        return await articleQueryService.FindBySlugAsync(slug, null);
    }

    public async Task<ArticleDataList> GetArticles(
        [Service] ArticleQueryService articleQueryService,
        string? tag = null,
        string? author = null,
        string? favoritedBy = null,
        int offset = 0,
        int limit = 20)
    {
        var page = new Page(offset, limit);
        return await articleQueryService.FindRecentArticlesAsync(tag, author, favoritedBy, page, null);
    }

    public async Task<ProfileData?> GetProfile(
        [Service] ProfileQueryService profileQueryService,
        string username)
    {
        return await profileQueryService.FindByUsernameAsync(username, null);
    }

    public async Task<List<string>> GetTags(
        [Service] TagsQueryService tagsQueryService)
    {
        return await tagsQueryService.AllTagsAsync();
    }
}
