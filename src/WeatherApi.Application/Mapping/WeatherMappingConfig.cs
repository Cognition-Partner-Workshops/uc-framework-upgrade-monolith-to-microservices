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
            .Map(dest => dest.Id, _ => Guid.NewGuid());

        config.NewConfig<UpdateWeatherForecastDto, WeatherForecast>()
            .IgnoreNullValues(true);
    }
}
