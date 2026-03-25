using Dapper;
using WeatherApi.Application.Common.Interfaces;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT Id, Username, PasswordHash, Email, Role, CreatedAt FROM Users WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT Id, Username, PasswordHash, Email, Role, CreatedAt FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT Id, Username, PasswordHash, Email, Role, CreatedAt FROM Users";
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt)
            VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}
