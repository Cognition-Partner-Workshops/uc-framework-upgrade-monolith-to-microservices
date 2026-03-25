using Mapster;
using WeatherApi.Application.DTOs;
using WeatherApi.Domain.Entities;

namespace WeatherApi.Application.Mapping;

public class WeatherMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WeatherForecast, WeatherForecastDto>();

        config.NewConfig<CreateWeatherForecastDto, WeatherForecast>()
            .Ignore(dest => dest.Id);

        config.NewConfig<UpdateWeatherForecastDto, WeatherForecast>()
            .Ignore(dest => dest.Id);
    }
}
