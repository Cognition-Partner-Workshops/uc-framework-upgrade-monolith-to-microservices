using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.Exceptions;
using RealWorld.Api.Security;
using RealWorld.Application.Services;
using RealWorld.Core.Interfaces;
using RealWorld.Infrastructure.Data;
using RealWorld.Infrastructure.Repositories;
using RealWorld.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=realworld.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));

// Repositories (write-side)
builder.Services.AddScoped<IArticleRepository, EfArticleRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IArticleFavoriteRepository, EfArticleFavoriteRepository>();

// Read-side services (Dapper)
builder.Services.AddScoped<IArticleReadService, DapperArticleReadService>();
builder.Services.AddScoped<ICommentReadService, DapperCommentReadService>();
builder.Services.AddScoped<IUserReadService, DapperUserReadService>();
builder.Services.AddScoped<IUserRelationshipQueryService, DapperUserRelationshipQueryService>();
builder.Services.AddScoped<IArticleFavoritesReadService, DapperArticleFavoritesReadService>();
builder.Services.AddScoped<ITagReadService, DapperTagReadService>();

// Application services
builder.Services.AddScoped<ArticleQueryService>();
builder.Services.AddScoped<CommentQueryService>();
builder.Services.AddScoped<UserQueryService>();
builder.Services.AddScoped<ProfileQueryService>();
builder.Services.AddScoped<TagsQueryService>();
builder.Services.AddScoped<ArticleCommandService>();
builder.Services.AddScoped<UserService>(sp =>
{
    var userRepo = sp.GetRequiredService<IUserRepository>();
    var defaultImage = builder.Configuration["image:default"] ?? "https://static.productionready.io/images/smiley-cyrus.jpg";
    return new UserService(userRepo, defaultImage);
});

// JWT
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<JwtTokenFilter>();

// Auth is handled by JwtTokenFilter middleware (no ASP.NET Core JWT Bearer needed)
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// GraphQL (Hot Chocolate)
builder.Services
    .AddGraphQLServer()
    .AddQueryType<RealWorld.Api.GraphQL.Query>()
    .AddMutationType<RealWorld.Api.GraphQL.Mutation>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    DatabaseSeeder.Seed(context);
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseExceptionHandler(_ => { });
app.UseMiddleware<JwtTokenFilter>();
app.MapControllers();
app.MapGraphQL();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
