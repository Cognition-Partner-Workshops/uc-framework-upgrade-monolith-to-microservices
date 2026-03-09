using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;
using RealWorld.Core.Entities;

namespace RealWorld.Application.Services;

public class ArticleQueryService
{
    private readonly IArticleReadService _articleReadService;
    private readonly IUserRelationshipQueryService _userRelationshipQueryService;
    private readonly IArticleFavoritesReadService _articleFavoritesReadService;

    public ArticleQueryService(
        IArticleReadService articleReadService,
        IUserRelationshipQueryService userRelationshipQueryService,
        IArticleFavoritesReadService articleFavoritesReadService)
    {
        _articleReadService = articleReadService;
        _userRelationshipQueryService = userRelationshipQueryService;
        _articleFavoritesReadService = articleFavoritesReadService;
    }

    public async Task<ArticleData?> FindByIdAsync(string id, User? user)
    {
        var articleData = await _articleReadService.FindByIdAsync(id);
        if (articleData == null)
            return null;

        if (user != null)
            await FillExtraInfoAsync(id, user, articleData);

        return articleData;
    }

    public async Task<ArticleData?> FindBySlugAsync(string slug, User? user)
    {
        var articleData = await _articleReadService.FindBySlugAsync(slug);
        if (articleData == null)
            return null;

        if (user != null)
            await FillExtraInfoAsync(articleData.Id, user, articleData);

        return articleData;
    }

    public async Task<CursorPager<ArticleDataNode>> FindRecentArticlesWithCursorAsync(
        string? tag, string? author, string? favoritedBy,
        CursorPageParameter<DateTimeOffset> page, User? currentUser)
    {
        var articleIds = await _articleReadService.FindArticlesWithCursorAsync(tag, author, favoritedBy, page);
        if (articleIds.Count == 0)
            return new CursorPager<ArticleDataNode>(new List<ArticleDataNode>(), page.Direction, false);

        bool hasExtra = articleIds.Count > page.Limit;
        if (hasExtra)
            articleIds.RemoveAt(page.Limit);

        if (!page.IsNext())
            articleIds.Reverse();

        var articles = await _articleReadService.FindArticlesAsync(articleIds);
        await FillExtraInfoAsync(articles, currentUser);

        return new CursorPager<ArticleDataNode>(
            articles.Select(a => new ArticleDataNode(a)).ToList(),
            page.Direction, hasExtra);
    }

    public async Task<CursorPager<ArticleDataNode>> FindUserFeedWithCursorAsync(
        User user, CursorPageParameter<DateTimeOffset> page)
    {
        var followedUsers = await _userRelationshipQueryService.FollowedUsersAsync(user.Id);
        if (followedUsers.Count == 0)
            return new CursorPager<ArticleDataNode>(new List<ArticleDataNode>(), page.Direction, false);

        var articles = await _articleReadService.FindArticlesOfAuthorsWithCursorAsync(followedUsers, page);
        bool hasExtra = articles.Count > page.Limit;
        if (hasExtra)
            articles.RemoveAt(page.Limit);

        if (!page.IsNext())
            articles.Reverse();

        await FillExtraInfoAsync(articles, user);
        return new CursorPager<ArticleDataNode>(
            articles.Select(a => new ArticleDataNode(a)).ToList(),
            page.Direction, hasExtra);
    }

    public async Task<ArticleDataList> FindRecentArticlesAsync(
        string? tag, string? author, string? favoritedBy, Page page, User? currentUser)
    {
        var articleIds = await _articleReadService.QueryArticlesAsync(tag, author, favoritedBy, page);
        int articleCount = await _articleReadService.CountArticleAsync(tag, author, favoritedBy);
        if (articleIds.Count == 0)
            return new ArticleDataList(new List<ArticleData>(), articleCount);

        var articles = await _articleReadService.FindArticlesAsync(articleIds);
        await FillExtraInfoAsync(articles, currentUser);
        return new ArticleDataList(articles, articleCount);
    }

    public async Task<ArticleDataList> FindUserFeedAsync(User user, Page page)
    {
        var followedUsers = await _userRelationshipQueryService.FollowedUsersAsync(user.Id);
        if (followedUsers.Count == 0)
            return new ArticleDataList(new List<ArticleData>(), 0);

        var articles = await _articleReadService.FindArticlesOfAuthorsAsync(followedUsers, page);
        await FillExtraInfoAsync(articles, user);
        int count = await _articleReadService.CountFeedSizeAsync(followedUsers);
        return new ArticleDataList(articles, count);
    }

    private async Task FillExtraInfoAsync(List<ArticleData> articles, User? currentUser)
    {
        await SetFavoriteCountAsync(articles);
        if (currentUser != null)
        {
            await SetIsFavoriteAsync(articles, currentUser);
            await SetIsFollowingAuthorAsync(articles, currentUser);
        }
    }

    private async Task SetIsFollowingAuthorAsync(List<ArticleData> articles, User currentUser)
    {
        var authorIds = articles.Select(a => a.ProfileData.Id).ToList();
        var followingAuthors = await _userRelationshipQueryService.FollowingAuthorsAsync(currentUser.Id, authorIds);
        foreach (var article in articles)
        {
            if (followingAuthors.Contains(article.ProfileData.Id))
                article.ProfileData.Following = true;
        }
    }

    private async Task SetFavoriteCountAsync(List<ArticleData> articles)
    {
        var ids = articles.Select(a => a.Id).ToList();
        var favoriteCounts = await _articleFavoritesReadService.ArticlesFavoriteCountAsync(ids);
        var countMap = favoriteCounts.ToDictionary(f => f.Id, f => (int)f.Count);
        foreach (var article in articles)
        {
            if (countMap.TryGetValue(article.Id, out var count))
                article.FavoritesCount = count;
        }
    }

    private async Task SetIsFavoriteAsync(List<ArticleData> articles, User currentUser)
    {
        var ids = articles.Select(a => a.Id).ToList();
        var favoritedArticles = await _articleFavoritesReadService.UserFavoritesAsync(ids, currentUser);
        foreach (var article in articles)
        {
            if (favoritedArticles.Contains(article.Id))
                article.Favorited = true;
        }
    }

    private async Task FillExtraInfoAsync(string id, User user, ArticleData articleData)
    {
        articleData.Favorited = await _articleFavoritesReadService.IsUserFavoriteAsync(user.Id, id);
        articleData.FavoritesCount = await _articleFavoritesReadService.ArticleFavoriteCountAsync(id);
        articleData.ProfileData.Following = await _userRelationshipQueryService.IsUserFollowingAsync(
            user.Id, articleData.ProfileData.Id);
    }
}

// Wrapper to implement INode for ArticleData
public class ArticleDataNode : INode
{
    public ArticleData Data { get; }

    public ArticleDataNode(ArticleData data)
    {
        Data = data;
    }

    public DateTimeCursor GetCursor()
    {
        return new DateTimeCursor(Data.UpdatedAt);
    }
}
