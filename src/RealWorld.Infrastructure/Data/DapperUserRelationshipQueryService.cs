using System.Data;
using Dapper;
using RealWorld.Application.Services;

namespace RealWorld.Infrastructure.Data;

public class DapperUserRelationshipQueryService : IUserRelationshipQueryService
{
    private readonly IDbConnection _connection;

    public DapperUserRelationshipQueryService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<bool> IsUserFollowingAsync(string userId, string anotherUserId)
    {
        var sql = "SELECT COUNT(1) FROM follows WHERE user_id = @UserId AND follow_id = @AnotherUserId";
        var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, AnotherUserId = anotherUserId });
        return count > 0;
    }

    public async Task<HashSet<string>> FollowingAuthorsAsync(string userId, List<string> ids)
    {
        if (ids.Count == 0) return new HashSet<string>();

        var sql = "SELECT follow_id FROM follows WHERE user_id = @UserId AND follow_id IN @Ids";
        var result = await _connection.QueryAsync<string>(sql, new { UserId = userId, Ids = ids });
        return result.ToHashSet();
    }

    public async Task<List<string>> FollowedUsersAsync(string userId)
    {
        var sql = "SELECT follow_id FROM follows WHERE user_id = @UserId";
        var result = await _connection.QueryAsync<string>(sql, new { UserId = userId });
        return result.ToList();
    }
}
