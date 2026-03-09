using Microsoft.EntityFrameworkCore;
using RealWorld.Api.Models;

namespace RealWorld.Api.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Users.Any()) return;

        // BCrypt hash for "password123"
        var passwordHash = "$2a$10$AbglDchyhkogGBIxNoHdN.pBDK86VNXtF.Vh6N72G9s1rjw7z2b4u";

        var users = new[]
        {
            new User { Id = "user-1", Username = "johndoe", Email = "john@example.com", Password = passwordHash, Bio = "Full-stack developer and tech enthusiast", Image = "https://api.dicebear.com/7.x/avataaars/svg?seed=John" },
            new User { Id = "user-2", Username = "janedoe", Email = "jane@example.com", Password = passwordHash, Bio = "Software architect passionate about clean code", Image = "https://api.dicebear.com/7.x/avataaars/svg?seed=Jane" },
            new User { Id = "user-3", Username = "bobsmith", Email = "bob@example.com", Password = passwordHash, Bio = "DevOps engineer and cloud enthusiast", Image = "https://api.dicebear.com/7.x/avataaars/svg?seed=Bob" }
        };
        context.Users.AddRange(users);

        var tags = new[]
        {
            new Tag { Id = "tag-1", Name = "java" },
            new Tag { Id = "tag-2", Name = "spring-boot" },
            new Tag { Id = "tag-3", Name = "web-development" },
            new Tag { Id = "tag-4", Name = "tutorial" },
            new Tag { Id = "tag-5", Name = "best-practices" },
            new Tag { Id = "tag-6", Name = "microservices" },
            new Tag { Id = "tag-7", Name = "api-design" }
        };
        context.Tags.AddRange(tags);

        var now = DateTime.UtcNow;
        var articles = new[]
        {
            new Article { Id = "article-1", UserId = "user-1", Slug = "getting-started-with-spring-boot", Title = "Getting Started with Spring Boot", Description = "A comprehensive guide to building your first Spring Boot application", Body = "Spring Boot makes it easy to create stand-alone, production-grade Spring based Applications that you can \"just run\".", CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-7) },
            new Article { Id = "article-2", UserId = "user-2", Slug = "rest-api-best-practices", Title = "REST API Best Practices", Description = "Learn the essential principles for designing robust REST APIs", Body = "Building a great REST API requires more than just exposing endpoints.", CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5) },
            new Article { Id = "article-3", UserId = "user-1", Slug = "microservices-architecture-guide", Title = "Microservices Architecture Guide", Description = "Understanding microservices patterns and when to use them", Body = "Microservices architecture has become increasingly popular, but it's not a silver bullet.", CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-3) },
            new Article { Id = "article-4", UserId = "user-3", Slug = "docker-for-java-developers", Title = "Docker for Java Developers", Description = "Containerize your Java applications with Docker", Body = "Docker has revolutionized how we deploy applications.", CreatedAt = now.AddDays(-2), UpdatedAt = now.AddDays(-2) },
            new Article { Id = "article-5", UserId = "user-2", Slug = "testing-spring-boot-applications", Title = "Testing Spring Boot Applications", Description = "A complete guide to testing strategies in Spring Boot", Body = "Testing is crucial for maintaining code quality.", CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1) }
        };
        context.Articles.AddRange(articles);

        var articleTags = new[]
        {
            new ArticleTag { ArticleId = "article-1", TagId = "tag-1" },
            new ArticleTag { ArticleId = "article-1", TagId = "tag-2" },
            new ArticleTag { ArticleId = "article-1", TagId = "tag-4" },
            new ArticleTag { ArticleId = "article-2", TagId = "tag-3" },
            new ArticleTag { ArticleId = "article-2", TagId = "tag-5" },
            new ArticleTag { ArticleId = "article-2", TagId = "tag-7" },
            new ArticleTag { ArticleId = "article-3", TagId = "tag-2" },
            new ArticleTag { ArticleId = "article-3", TagId = "tag-6" },
            new ArticleTag { ArticleId = "article-3", TagId = "tag-5" },
            new ArticleTag { ArticleId = "article-4", TagId = "tag-1" },
            new ArticleTag { ArticleId = "article-4", TagId = "tag-2" },
            new ArticleTag { ArticleId = "article-4", TagId = "tag-4" },
            new ArticleTag { ArticleId = "article-5", TagId = "tag-1" },
            new ArticleTag { ArticleId = "article-5", TagId = "tag-2" },
            new ArticleTag { ArticleId = "article-5", TagId = "tag-5" }
        };
        context.ArticleTags.AddRange(articleTags);

        var favorites = new[]
        {
            new ArticleFavorite { ArticleId = "article-1", UserId = "user-2" },
            new ArticleFavorite { ArticleId = "article-1", UserId = "user-3" },
            new ArticleFavorite { ArticleId = "article-2", UserId = "user-1" },
            new ArticleFavorite { ArticleId = "article-3", UserId = "user-2" },
            new ArticleFavorite { ArticleId = "article-4", UserId = "user-1" },
            new ArticleFavorite { ArticleId = "article-5", UserId = "user-3" }
        };
        context.ArticleFavorites.AddRange(favorites);

        var follows = new[]
        {
            new FollowRelation { UserId = "user-1", TargetId = "user-2" },
            new FollowRelation { UserId = "user-2", TargetId = "user-1" },
            new FollowRelation { UserId = "user-3", TargetId = "user-1" },
            new FollowRelation { UserId = "user-3", TargetId = "user-2" }
        };
        context.Follows.AddRange(follows);

        var comments = new[]
        {
            new Comment { Id = "comment-1", Body = "Great article! This really helped me understand Spring Boot basics.", ArticleId = "article-1", UserId = "user-2", CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-6) },
            new Comment { Id = "comment-2", Body = "Thanks for sharing. The code examples are very clear.", ArticleId = "article-1", UserId = "user-3", CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-6) },
            new Comment { Id = "comment-3", Body = "Excellent best practices guide. I'll be implementing these in my project.", ArticleId = "article-2", UserId = "user-1", CreatedAt = now.AddDays(-4), UpdatedAt = now.AddDays(-4) },
            new Comment { Id = "comment-4", Body = "Very comprehensive overview of microservices. Well written!", ArticleId = "article-3", UserId = "user-2", CreatedAt = now.AddDays(-2), UpdatedAt = now.AddDays(-2) },
            new Comment { Id = "comment-5", Body = "Docker tutorial was exactly what I needed. Thanks!", ArticleId = "article-4", UserId = "user-1", CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1) }
        };
        context.Comments.AddRange(comments);

        context.SaveChanges();
    }
}
