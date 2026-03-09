using Microsoft.EntityFrameworkCore;

namespace RealWorld.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // Check if already seeded
        if (context.Users.Any())
            return;

        // BCrypt hash for "password123"
        const string bcryptHash = "$2a$10$AbglDchyhkogGBIxNoHdN.pBDK86VNXtF.Vh6N72G9s1rjw7z2b4u";

        // Seed users
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO users (id, username, email, password, bio, image) VALUES
            ('user-1', 'johndoe', 'john@example.com', '{0}', 'Full-stack developer and tech enthusiast', 'https://api.dicebear.com/7.x/avataaars/svg?seed=John'),
            ('user-2', 'janedoe', 'jane@example.com', '{0}', 'Software architect passionate about clean code', 'https://api.dicebear.com/7.x/avataaars/svg?seed=Jane'),
            ('user-3', 'bobsmith', 'bob@example.com', '{0}', 'DevOps engineer and cloud enthusiast', 'https://api.dicebear.com/7.x/avataaars/svg?seed=Bob')
        ".Replace("{0}", bcryptHash));

        // Seed tags
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO tags (id, name) VALUES
            ('tag-1', 'java'),
            ('tag-2', 'spring-boot'),
            ('tag-3', 'web-development'),
            ('tag-4', 'tutorial'),
            ('tag-5', 'best-practices'),
            ('tag-6', 'microservices'),
            ('tag-7', 'api-design')
        ");

        // Seed articles
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO articles (id, user_id, slug, title, description, body, created_at, updated_at) VALUES
            ('article-1', 'user-1', 'getting-started-with-spring-boot', 'Getting Started with Spring Boot', 'A comprehensive guide to building your first Spring Boot application', 'Spring Boot makes it easy to create stand-alone, production-grade Spring based Applications that you can ""just run"". In this article, we will explore the fundamentals of Spring Boot and build a simple REST API.

## Prerequisites
- Java 11 or higher
- Basic understanding of Spring Framework
- Maven or Gradle

## Creating Your First Application
Start by visiting start.spring.io and selecting your dependencies...', datetime('now', '-7 days'), datetime('now', '-7 days')),

            ('article-2', 'user-2', 'rest-api-best-practices', 'REST API Best Practices', 'Learn the essential principles for designing robust REST APIs', 'Building a great REST API requires more than just exposing endpoints. In this article, we''ll cover the best practices that will make your API intuitive, maintainable, and scalable.

## Key Principles
1. Use proper HTTP methods
2. Implement consistent naming conventions
3. Version your API
4. Handle errors gracefully
5. Document everything

Let''s dive into each principle...', datetime('now', '-5 days'), datetime('now', '-5 days')),

            ('article-3', 'user-1', 'microservices-architecture-guide', 'Microservices Architecture Guide', 'Understanding microservices patterns and when to use them', 'Microservices architecture has become increasingly popular, but it''s not a silver bullet. This guide will help you understand when and how to implement microservices effectively.

## What are Microservices?
Microservices are an architectural style that structures an application as a collection of loosely coupled services...

## Benefits
- Independent deployment
- Technology diversity
- Fault isolation
- Scalability', datetime('now', '-3 days'), datetime('now', '-3 days')),

            ('article-4', 'user-3', 'docker-for-java-developers', 'Docker for Java Developers', 'Containerize your Java applications with Docker', 'Docker has revolutionized how we deploy applications. In this tutorial, we''ll learn how to containerize a Spring Boot application and deploy it using Docker.

## Why Docker?
- Consistent environments
- Easy deployment
- Isolation
- Portability

## Creating a Dockerfile
Here''s a simple Dockerfile for a Spring Boot app...', datetime('now', '-2 days'), datetime('now', '-2 days')),

            ('article-5', 'user-2', 'testing-spring-boot-applications', 'Testing Spring Boot Applications', 'A complete guide to testing strategies in Spring Boot', 'Testing is crucial for maintaining code quality. This article covers unit testing, integration testing, and end-to-end testing in Spring Boot applications.

## Testing Layers
1. Unit Tests with JUnit and Mockito
2. Integration Tests with @SpringBootTest
3. API Tests with MockMvc
4. Database Tests with @DataJpaTest

Let''s explore each testing strategy...', datetime('now', '-1 days'), datetime('now', '-1 days'))
        ");

        // Link articles to tags
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO article_tags (article_id, tag_id) VALUES
            ('article-1', 'tag-1'),
            ('article-1', 'tag-2'),
            ('article-1', 'tag-4'),
            ('article-2', 'tag-3'),
            ('article-2', 'tag-5'),
            ('article-2', 'tag-7'),
            ('article-3', 'tag-2'),
            ('article-3', 'tag-6'),
            ('article-3', 'tag-5'),
            ('article-4', 'tag-1'),
            ('article-4', 'tag-2'),
            ('article-4', 'tag-4'),
            ('article-5', 'tag-1'),
            ('article-5', 'tag-2'),
            ('article-5', 'tag-5')
        ");

        // Add favorites
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO article_favorites (article_id, user_id) VALUES
            ('article-1', 'user-2'),
            ('article-1', 'user-3'),
            ('article-2', 'user-1'),
            ('article-3', 'user-2'),
            ('article-4', 'user-1'),
            ('article-5', 'user-3')
        ");

        // Add follows
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO follows (user_id, follow_id) VALUES
            ('user-1', 'user-2'),
            ('user-2', 'user-1'),
            ('user-3', 'user-1'),
            ('user-3', 'user-2')
        ");

        // Add comments
        context.Database.ExecuteSqlRaw(@"
            INSERT INTO comments (id, body, article_id, user_id, created_at) VALUES
            ('comment-1', 'Great article! This really helped me understand Spring Boot basics.', 'article-1', 'user-2', datetime('now', '-6 days')),
            ('comment-2', 'Thanks for sharing. The code examples are very clear.', 'article-1', 'user-3', datetime('now', '-6 days')),
            ('comment-3', 'Excellent best practices guide. I''ll be implementing these in my project.', 'article-2', 'user-1', datetime('now', '-4 days')),
            ('comment-4', 'Very comprehensive overview of microservices. Well written!', 'article-3', 'user-2', datetime('now', '-2 days')),
            ('comment-5', 'Docker tutorial was exactly what I needed. Thanks!', 'article-4', 'user-1', datetime('now', '-1 days'))
        ");
    }
}
