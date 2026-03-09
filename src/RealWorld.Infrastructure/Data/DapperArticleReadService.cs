using System.Data;
using Dapper;
using RealWorld.Application.DTOs;
using RealWorld.Application.Pagination;
using RealWorld.Application.Services;

namespace RealWorld.Infrastructure.Data;

public class DapperArticleReadService : IArticleReadService
{
    private readonly IDbConnection _connection;

    public DapperArticleReadService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<ArticleData?> FindByIdAsync(string id)
    {
        var sql = @"
            SELECT A.id, A.slug, A.title, A.description, A.body,
                   A.created_at AS CreatedAt, A.updated_at AS UpdatedAt,
                   T.name AS TagName,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN users U ON U.id = A.user_id
            WHERE A.id = @Id";

        return await MapArticleDataAsync(sql, new { Id = id });
    }

    public async Task<ArticleData?> FindBySlugAsync(string slug)
    {
        var sql = @"
            SELECT A.id, A.slug, A.title, A.description, A.body,
                   A.created_at AS CreatedAt, A.updated_at AS UpdatedAt,
                   T.name AS TagName,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN users U ON U.id = A.user_id
            WHERE A.slug = @Slug";

        return await MapArticleDataAsync(sql, new { Slug = slug });
    }

    public async Task<List<string>> QueryArticlesAsync(string? tag, string? author, string? favoritedBy, Page page)
    {
        var sql = @"
            SELECT DISTINCT A.id
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN article_favorites AF ON AF.article_id = A.id
            LEFT JOIN users AU ON AU.id = A.user_id
            LEFT JOIN users AFU ON AFU.id = AF.user_id
            WHERE 1=1";

        var parameters = new DynamicParameters();
        if (tag != null) { sql += " AND T.name = @Tag"; parameters.Add("Tag", tag); }
        if (author != null) { sql += " AND AU.username = @Author"; parameters.Add("Author", author); }
        if (favoritedBy != null) { sql += " AND AFU.username = @FavoritedBy"; parameters.Add("FavoritedBy", favoritedBy); }

        sql += " ORDER BY A.created_at DESC LIMIT @Limit OFFSET @Offset";
        parameters.Add("Limit", page.Limit);
        parameters.Add("Offset", page.Offset);

        var result = await _connection.QueryAsync<string>(sql, parameters);
        return result.ToList();
    }

    public async Task<int> CountArticleAsync(string? tag, string? author, string? favoritedBy)
    {
        var sql = @"
            SELECT COUNT(DISTINCT A.id)
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN article_favorites AF ON AF.article_id = A.id
            LEFT JOIN users AU ON AU.id = A.user_id
            LEFT JOIN users AFU ON AFU.id = AF.user_id
            WHERE 1=1";

        var parameters = new DynamicParameters();
        if (tag != null) { sql += " AND T.name = @Tag"; parameters.Add("Tag", tag); }
        if (author != null) { sql += " AND AU.username = @Author"; parameters.Add("Author", author); }
        if (favoritedBy != null) { sql += " AND AFU.username = @FavoritedBy"; parameters.Add("FavoritedBy", favoritedBy); }

        return await _connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<List<ArticleData>> FindArticlesAsync(List<string> articleIds)
    {
        if (articleIds.Count == 0) return new List<ArticleData>();

        var sql = @"
            SELECT A.id, A.slug, A.title, A.description, A.body,
                   A.created_at AS CreatedAt, A.updated_at AS UpdatedAt,
                   T.name AS TagName,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN users U ON U.id = A.user_id
            WHERE A.id IN @Ids
            ORDER BY A.created_at DESC";

        return await MapArticleDataListAsync(sql, new { Ids = articleIds });
    }

    public async Task<List<ArticleData>> FindArticlesOfAuthorsAsync(List<string> authors, Page page)
    {
        if (authors.Count == 0) return new List<ArticleData>();

        var sql = @"
            SELECT A.id, A.slug, A.title, A.description, A.body,
                   A.created_at AS CreatedAt, A.updated_at AS UpdatedAt,
                   T.name AS TagName,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN users U ON U.id = A.user_id
            WHERE A.user_id IN @Authors
            LIMIT @Limit OFFSET @Offset";

        return await MapArticleDataListAsync(sql, new { Authors = authors, Limit = page.Limit, Offset = page.Offset });
    }

    public async Task<List<ArticleData>> FindArticlesOfAuthorsWithCursorAsync(List<string> authors, CursorPageParameter<DateTimeOffset> page)
    {
        if (authors.Count == 0) return new List<ArticleData>();

        var sql = @"
            SELECT A.id, A.slug, A.title, A.description, A.body,
                   A.created_at AS CreatedAt, A.updated_at AS UpdatedAt,
                   T.name AS TagName,
                   U.id AS UserId, U.username AS UserUsername, U.bio AS UserBio, U.image AS UserImage
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN users U ON U.id = A.user_id
            WHERE A.user_id IN @Authors";

        var parameters = new DynamicParameters();
        parameters.Add("Authors", authors);
        parameters.Add("QueryLimit", page.QueryLimit);

        if (page.Cursor != null && page.Direction == Direction.Next)
        {
            sql += " AND A.created_at < @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }
        else if (page.Cursor != null && page.Direction == Direction.Prev)
        {
            sql += " AND A.created_at > @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }

        sql += page.Direction == Direction.Next
            ? " ORDER BY A.created_at DESC"
            : " ORDER BY A.created_at ASC";
        sql += " LIMIT @QueryLimit";

        return await MapArticleDataListAsync(sql, parameters);
    }

    public async Task<int> CountFeedSizeAsync(List<string> authors)
    {
        if (authors.Count == 0) return 0;

        var sql = "SELECT COUNT(1) FROM articles A WHERE A.user_id IN @Authors";
        return await _connection.ExecuteScalarAsync<int>(sql, new { Authors = authors });
    }

    public async Task<List<string>> FindArticlesWithCursorAsync(string? tag, string? author, string? favoritedBy, CursorPageParameter<DateTimeOffset> page)
    {
        var sql = @"
            SELECT DISTINCT A.id, A.created_at
            FROM articles A
            LEFT JOIN article_tags AT2 ON A.id = AT2.article_id
            LEFT JOIN tags T ON T.id = AT2.tag_id
            LEFT JOIN article_favorites AF ON AF.article_id = A.id
            LEFT JOIN users AU ON AU.id = A.user_id
            LEFT JOIN users AFU ON AFU.id = AF.user_id
            WHERE 1=1";

        var parameters = new DynamicParameters();
        if (tag != null) { sql += " AND T.name = @Tag"; parameters.Add("Tag", tag); }
        if (author != null) { sql += " AND AU.username = @Author"; parameters.Add("Author", author); }
        if (favoritedBy != null) { sql += " AND AFU.username = @FavoritedBy"; parameters.Add("FavoritedBy", favoritedBy); }

        if (page.Cursor != null && page.Direction == Direction.Next)
        {
            sql += " AND A.created_at < @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }
        else if (page.Cursor != null && page.Direction == Direction.Prev)
        {
            sql += " AND A.created_at > @Cursor";
            parameters.Add("Cursor", page.Cursor.ToString());
        }

        sql += page.Direction == Direction.Next
            ? " ORDER BY A.created_at DESC"
            : " ORDER BY A.created_at ASC";

        parameters.Add("QueryLimit", page.QueryLimit);
        sql += " LIMIT @QueryLimit";

        var result = await _connection.QueryAsync<string>(sql, parameters);
        return result.ToList();
    }

    private async Task<ArticleData?> MapArticleDataAsync(string sql, object parameters)
    {
        var articleDict = new Dictionary<string, ArticleData>();
        var rows = await _connection.QueryAsync<ArticleRow>(sql, parameters);
        foreach (var row in rows)
        {
            if (!articleDict.TryGetValue(row.Id, out var article))
            {
                article = new ArticleData
                {
                    Id = row.Id,
                    Slug = row.Slug,
                    Title = row.Title,
                    Description = row.Description,
                    Body = row.Body,
                    CreatedAt = row.ParsedCreatedAt,
                    UpdatedAt = row.ParsedUpdatedAt,
                    TagList = new List<string>(),
                    ProfileData = new ProfileData
                    {
                        Id = row.UserId,
                        Username = row.UserUsername,
                        Bio = row.UserBio ?? "",
                        Image = row.UserImage ?? ""
                    }
                };
                articleDict[row.Id] = article;
            }
            if (!string.IsNullOrEmpty(row.TagName) && !article.TagList.Contains(row.TagName))
                article.TagList.Add(row.TagName);
        }

        return articleDict.Values.FirstOrDefault();
    }

    private async Task<List<ArticleData>> MapArticleDataListAsync(string sql, object parameters)
    {
        var articleDict = new Dictionary<string, ArticleData>();
        var order = new List<string>();

        var rows = await _connection.QueryAsync<ArticleRow>(sql, parameters);
        foreach (var row in rows)
        {
            if (!articleDict.TryGetValue(row.Id, out var article))
            {
                article = new ArticleData
                {
                    Id = row.Id,
                    Slug = row.Slug,
                    Title = row.Title,
                    Description = row.Description,
                    Body = row.Body,
                    CreatedAt = row.ParsedCreatedAt,
                    UpdatedAt = row.ParsedUpdatedAt,
                    TagList = new List<string>(),
                    ProfileData = new ProfileData
                    {
                        Id = row.UserId,
                        Username = row.UserUsername,
                        Bio = row.UserBio ?? "",
                        Image = row.UserImage ?? ""
                    }
                };
                articleDict[row.Id] = article;
                order.Add(row.Id);
            }
            if (!string.IsNullOrEmpty(row.TagName) && !article.TagList.Contains(row.TagName))
                article.TagList.Add(row.TagName);
        }

        return order.Select(id => articleDict[id]).ToList();
    }

    private class ArticleRow
    {
        public string Id { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string? TagName { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserUsername { get; set; } = string.Empty;
        public string? UserBio { get; set; }
        public string? UserImage { get; set; }

        public DateTimeOffset ParsedCreatedAt => DateTimeOffset.TryParse(CreatedAt, out var dt) ? dt : DateTimeOffset.MinValue;
        public DateTimeOffset ParsedUpdatedAt => DateTimeOffset.TryParse(UpdatedAt, out var dt) ? dt : DateTimeOffset.MinValue;
    }
}
