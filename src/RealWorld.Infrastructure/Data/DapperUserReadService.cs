using System.Data;
using Dapper;
using RealWorld.Application.DTOs;
using RealWorld.Application.Services;

namespace RealWorld.Infrastructure.Data;

public class DapperUserReadService : IUserReadService
{
    private readonly IDbConnection _connection;

    public DapperUserReadService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<UserData?> FindByUsernameAsync(string username)
    {
        var sql = "SELECT id AS Id, email AS Email, username AS Username, bio AS Bio, image AS Image FROM users WHERE username = @Username";
        return await _connection.QueryFirstOrDefaultAsync<UserData>(sql, new { Username = username });
    }

    public async Task<UserData?> FindByIdAsync(string id)
    {
        var sql = "SELECT id AS Id, email AS Email, username AS Username, bio AS Bio, image AS Image FROM users WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<UserData>(sql, new { Id = id });
    }
}
