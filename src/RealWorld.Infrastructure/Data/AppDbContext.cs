using Microsoft.EntityFrameworkCore;
using RealWorld.Core.Entities;

namespace RealWorld.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArticleFavorite> ArticleFavorites => Set<ArticleFavorite>();
    public DbSet<FollowRelation> FollowRelations => Set<FollowRelation>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Users table
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(255);
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(255);
            entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Image).HasColumnName("image").HasMaxLength(511);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Articles table
        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("articles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(255);
            entity.Property(e => e.Slug).HasColumnName("slug").HasMaxLength(255);
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Ignore(e => e.Tags);
        });

        // Tags table
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(255);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
        });

        // article_tags join table
        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.ToTable("article_tags");
            entity.HasKey(e => new { e.ArticleId, e.TagId });
            entity.Property(e => e.ArticleId).HasColumnName("article_id").HasMaxLength(255);
            entity.Property(e => e.TagId).HasColumnName("tag_id").HasMaxLength(255);
        });

        // article_favorites table
        modelBuilder.Entity<ArticleFavorite>(entity =>
        {
            entity.ToTable("article_favorites");
            entity.HasKey(e => new { e.ArticleId, e.UserId });
            entity.Property(e => e.ArticleId).HasColumnName("article_id").HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(255);
        });

        // follows table
        modelBuilder.Entity<FollowRelation>(entity =>
        {
            entity.ToTable("follows");
            entity.HasKey(e => new { e.UserId, e.TargetId });
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(255);
            entity.Property(e => e.TargetId).HasColumnName("follow_id").HasMaxLength(255);
        });

        // comments table
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(255);
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.ArticleId).HasColumnName("article_id").HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }
}

// Shadow entity for the article_tags join table
public class ArticleTag
{
    public string ArticleId { get; set; } = string.Empty;
    public string TagId { get; set; } = string.Empty;
}
