using System.Data;
using Dapper;
using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;
using RealWorld.Application.Services;

namespace RealWorld.Infrastructure.Data;

public class DapperCommentReadService : ICommentReadService
{
    private readonly IDbConnection _connection;

    public DapperCommentReadService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<CommentData?> FindByIdAsync(string id)
    {
        var sql = @"
            SELECT C.id, C.body, C.article_id AS ArticleId,
                   C.created_at AS CreatedAt, C.created_at AS UpdatedAt,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM comments C
            LEFT JOIN users U ON U.id = C.user_id
            WHERE C.id = @Id";

        var rows = await _connection.QueryAsync<CommentRow>(sql, new { Id = id });
        var row = rows.FirstOrDefault();
        if (row == null) return null;

        return MapToCommentData(row);
    }

    public async Task<List<CommentData>> FindByArticleIdAsync(string articleId)
    {
        var sql = @"
            SELECT C.id, C.body, C.article_id AS ArticleId,
                   C.created_at AS CreatedAt, C.created_at AS UpdatedAt,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM comments C
            LEFT JOIN users U ON U.id = C.user_id
            WHERE C.article_id = @ArticleId
            ORDER BY C.created_at DESC";

        var rows = await _connection.QueryAsync<CommentRow>(sql, new { ArticleId = articleId });
        return rows.Select(MapToCommentData).ToList();
    }

    public async Task<List<CommentData>> FindByArticleIdWithCursorAsync(string articleId, CursorPageParameter<DateTimeOffset> page)
    {
        var sql = @"
            SELECT C.id, C.body, C.article_id AS ArticleId,
                   C.created_at AS CreatedAt, C.created_at AS UpdatedAt,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM comments C
            LEFT JOIN users U ON U.id = C.user_id
            WHERE C.article_id = @ArticleId";

        var parameters = new DynamicParameters();
        parameters.Add("ArticleId", articleId);
        parameters.Add("QueryLimit", page.QueryLimit);

        if (page.Cursor != null && page.Direction == Direction.Next)
        {
            sql += " AND C.created_at < @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }
        else if (page.Cursor != null && page.Direction == Direction.Prev)
        {
            sql += " AND C.created_at > @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }

        sql += page.Direction == Direction.Next
            ? " ORDER BY C.created_at DESC"
            : " ORDER BY C.created_at ASC";
        sql += " LIMIT @QueryLimit";

        var rows = await _connection.QueryAsync<CommentRow>(sql, parameters);
        return rows.Select(MapToCommentData).ToList();
    }

    private static CommentData MapToCommentData(CommentRow row)
    {
        return new CommentData
        {
            Id = row.Id,
            Body = row.Body,
            ArticleId = row.ArticleId,
            CreatedAt = row.ParsedCreatedAt,
            UpdatedAt = row.ParsedUpdatedAt,
            ProfileData = new ProfileData
            {
                Id = row.UserId,
                Username = row.UserUsername,
                Bio = row.UserBio ?? "",
                Image = row.UserImage ?? ""
            }
        };
    }

    private class CommentRow
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ArticleId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserUsername { get; set; } = string.Empty;
        public string? UserBio { get; set; }
        public string? UserImage { get; set; }

        public DateTimeOffset ParsedCreatedAt => DateTimeOffset.TryParse(CreatedAt, out var dt) ? dt : DateTimeOffset.MinValue;
        public DateTimeOffset ParsedUpdatedAt => DateTimeOffset.TryParse(UpdatedAt, out var dt) ? dt : DateTimeOffset.MinValue;
    }
}
