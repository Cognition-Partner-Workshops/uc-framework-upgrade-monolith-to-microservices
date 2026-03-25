using MapsterMapper;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Application.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _repository;
    private readonly IMapper _mapper;

    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private static readonly string[] Cities =
    {
        "New York", "London", "Tokyo", "Sydney", "Paris",
        "Berlin", "Toronto", "Mumbai", "São Paulo", "Cairo"
    };

    public WeatherForecastService(IWeatherForecastRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetAllAsync()
    {
        var forecasts = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<WeatherForecastDto>>(forecasts);
    }

    public async Task<WeatherForecastDto?> GetByIdAsync(Guid id)
    {
        var forecast = await _repository.GetByIdAsync(id);
        return forecast is null ? null : _mapper.Map<WeatherForecastDto>(forecast);
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetByCityAsync(string city)
    {
        var forecasts = await _repository.GetByCityAsync(city);
        return _mapper.Map<IEnumerable<WeatherForecastDto>>(forecasts);
    }

    public async Task<WeatherForecastDto> CreateAsync(CreateWeatherForecastDto dto)
    {
        var entity = _mapper.Map<WeatherForecast>(dto);
        entity.Id = Guid.NewGuid();
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<WeatherForecastDto>(created);
    }

    public async Task<WeatherForecastDto?> UpdateAsync(Guid id, UpdateWeatherForecastDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.City = dto.City;
        existing.Date = dto.Date;
        existing.TemperatureCelsius = dto.TemperatureCelsius;
        existing.Summary = dto.Summary;
        existing.Humidity = dto.Humidity;
        existing.WindSpeedKmh = dto.WindSpeedKmh;

        var updated = await _repository.UpdateAsync(existing);
        return updated is null ? null : _mapper.Map<WeatherForecastDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<WeatherForecastDto> GenerateRandomAsync(string? city = null)
    {
        var random = new Random();

        var forecast = new WeatherForecast
        {
            Id = Guid.NewGuid(),
            City = city ?? Cities[random.Next(Cities.Length)],
            Date = DateTime.UtcNow.AddDays(random.Next(1, 15)),
            TemperatureCelsius = Math.Round(random.NextDouble() * 60 - 20, 1),
            Summary = Summaries[random.Next(Summaries.Length)],
            Humidity = random.Next(10, 100),
            WindSpeedKmh = Math.Round(random.NextDouble() * 120, 1)
        };

        var created = await _repository.AddAsync(forecast);
        return _mapper.Map<WeatherForecastDto>(created);
    }
}
