using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecastDto>> GetAllAsync();
    Task<WeatherForecastDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<WeatherForecastDto>> GetByCityAsync(string city);
    Task<WeatherForecastDto> CreateAsync(CreateWeatherForecastDto dto);
    Task<WeatherForecastDto?> UpdateAsync(Guid id, UpdateWeatherForecastDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<WeatherForecastDto> GenerateRandomAsync(string? city = null);
}
