using WeatherApi.Domain.Entities;

namespace WeatherApi.Domain.Interfaces;

public interface IWeatherForecastRepository
{
    Task<IEnumerable<WeatherForecast>> GetAllAsync();
    Task<WeatherForecast?> GetByIdAsync(Guid id);
    Task<IEnumerable<WeatherForecast>> GetByCityAsync(string city);
    Task<WeatherForecast> AddAsync(WeatherForecast forecast);
    Task<WeatherForecast?> UpdateAsync(WeatherForecast forecast);
    Task<bool> DeleteAsync(Guid id);
}
