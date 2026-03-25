using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Domain.Interfaces;
using WeatherApi.Infrastructure.Data;
using WeatherApi.Infrastructure.Repositories;

namespace WeatherApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseInMemoryDatabase("WeatherDb"));

        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}
