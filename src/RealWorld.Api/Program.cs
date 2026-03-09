using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

var jwtSecret = builder.Configuration["jwt:secret"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "CHANGE_ME_USE_ENV_VAR_JWT_SECRET_IN_PRODUCTION";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key
        };
        // Support "Token <jwt>" header format (RealWorld spec)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var header = context.Request.Headers["Authorization"].FirstOrDefault();
                if (header != null && header.StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = header[6..].Trim();
                }
                return Task.CompletedTask;
            }
        };
    });

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
