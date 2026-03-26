using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApi.Application.Common.Interfaces;

namespace WeatherApi.Infrastructure.Persistence;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
