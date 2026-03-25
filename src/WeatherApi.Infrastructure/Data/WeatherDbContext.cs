using Microsoft.EntityFrameworkCore;
using WeatherApi.Domain.Entities;

namespace WeatherApi.Infrastructure.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Summary).HasMaxLength(200);
            entity.Ignore(e => e.TemperatureFahrenheit);
        });

        base.OnModelCreating(modelBuilder);
    }
}
