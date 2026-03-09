using System.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealWorld.Infrastructure.Data;

namespace RealWorld.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTags_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/tags");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetArticles_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/articles");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}: {body}");
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sharedConnection;

    public CustomWebApplicationFactory()
    {
        _sharedConnection = new SqliteConnection("Data Source=IntegrationTestDb;Mode=Memory;Cache=Shared");
        _sharedConnection.Open();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext and IDbConnection registrations
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnection));
            if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

            // Use shared in-memory SQLite for both EF Core and Dapper
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_sharedConnection));

            services.AddScoped<IDbConnection>(_ =>
            {
                var conn = new SqliteConnection("Data Source=IntegrationTestDb;Mode=Memory;Cache=Shared");
                conn.Open();
                return conn;
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _sharedConnection.Dispose();
    }
}
