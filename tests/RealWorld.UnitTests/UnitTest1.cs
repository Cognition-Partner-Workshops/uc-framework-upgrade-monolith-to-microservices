using FluentAssertions;
using RealWorld.Core.Entities;

namespace RealWorld.UnitTests;

public class ArticleTests
{
    [Fact]
    public void Article_Constructor_ShouldGenerateSlug()
    {
        var article = new Article("Getting Started with Spring Boot", "description", "body", new List<string> { "java" }, "user-1");
        article.Slug.Should().Be("getting-started-with-spring-boot");
    }

    [Fact]
    public void Article_Constructor_ShouldGenerateId()
    {
        var article = new Article("Test Title", "desc", "body", new List<string>(), "user-1");
        article.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Article_Update_ShouldOnlyUpdateNonEmptyFields()
    {
        var article = new Article("Original Title", "Original Description", "Original Body", new List<string>(), "user-1");
        article.Update("New Title", "", "");
        article.Title.Should().Be("New Title");
        article.Description.Should().Be("Original Description");
        article.Body.Should().Be("Original Body");
    }

    [Fact]
    public void Article_ToSlug_ShouldConvertCorrectly()
    {
        var slug = Article.ToSlug("Hello World & Goodbye");
        slug.Should().Be("hello-world-goodbye");
    }
}

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldGenerateId()
    {
        var user = new User("test@example.com", "testuser", "hashedpw", "", "");
        user.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void User_Update_ShouldOnlyUpdateNonEmptyFields()
    {
        var user = new User("test@example.com", "testuser", "hashedpw", "bio", "image");
        user.Update("new@example.com", "", "", "", "");
        user.Email.Should().Be("new@example.com");
        user.Username.Should().Be("testuser");
    }
}

public class TagTests
{
    [Fact]
    public void Tag_Constructor_ShouldGenerateId()
    {
        var tag = new Tag("java");
        tag.Id.Should().NotBeNullOrEmpty();
        tag.Name.Should().Be("java");
    }
}

public class CommentTests
{
    [Fact]
    public void Comment_Constructor_ShouldSetCreatedAt()
    {
        var comment = new Comment("Great article!", "user-1", "article-1");
        comment.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
