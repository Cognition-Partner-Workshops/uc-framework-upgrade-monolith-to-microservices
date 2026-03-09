using System.Data;
using Dapper;
using RealWorld.Application.DTOs;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;

namespace RealWorld.Infrastructure.Data;

public class DapperArticleFavoritesReadService : IArticleFavoritesReadService
{
    private readonly IDbConnection _connection;

    public DapperArticleFavoritesReadService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<bool> IsUserFavoriteAsync(string userId, string articleId)
    {
        var sql = "SELECT COUNT(1) FROM article_favorites WHERE user_id = @UserId AND article_id = @ArticleId";
        var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ArticleId = articleId });
        return count > 0;
    }

    public async Task<int> ArticleFavoriteCountAsync(string articleId)
    {
        var sql = "SELECT COUNT(1) FROM article_favorites WHERE article_id = @ArticleId";
        return await _connection.ExecuteScalarAsync<int>(sql, new { ArticleId = articleId });
    }

    public async Task<List<ArticleFavoriteCount>> ArticlesFavoriteCountAsync(List<string> ids)
    {
        if (ids.Count == 0) return new List<ArticleFavoriteCount>();

        var sql = "SELECT article_id AS Id, COUNT(1) AS Count FROM article_favorites WHERE article_id IN @Ids GROUP BY article_id";
        var result = await _connection.QueryAsync<ArticleFavoriteCount>(sql, new { Ids = ids });
        return result.ToList();
    }

    public async Task<HashSet<string>> UserFavoritesAsync(List<string> ids, User currentUser)
    {
        if (ids.Count == 0) return new HashSet<string>();

        var sql = "SELECT article_id FROM article_favorites WHERE article_id IN @Ids AND user_id = @UserId";
        var result = await _connection.QueryAsync<string>(sql, new { Ids = ids, UserId = currentUser.Id });
        return result.ToHashSet();
    }
}
