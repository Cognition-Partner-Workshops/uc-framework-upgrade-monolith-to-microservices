using System.Data;
using Dapper;
using RealWorld.Application.Services;

namespace RealWorld.Infrastructure.Data;

public class DapperTagReadService : ITagReadService
{
    private readonly IDbConnection _connection;

    public DapperTagReadService(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();
    }

    public async Task<List<string>> AllAsync()
    {
        var sql = "SELECT name FROM tags";
        var result = await _connection.QueryAsync<string>(sql);
        return result.ToList();
    }
}
