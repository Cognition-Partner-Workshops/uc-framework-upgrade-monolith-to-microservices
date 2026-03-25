using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Application.Interfaces;
using WeatherApi.Application.Services;

namespace WeatherApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        return services;
    }
}
