using System.Data;
using Microsoft.Data.Sqlite;

namespace CqrsApi.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=cqrsapi.db";
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
