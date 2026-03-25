using Microsoft.EntityFrameworkCore;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interfaces;
using WeatherApi.Infrastructure.Data;

namespace WeatherApi.Infrastructure.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly WeatherDbContext _context;

    public WeatherForecastRepository(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeatherForecast>> GetAllAsync()
    {
        return await _context.WeatherForecasts.ToListAsync();
    }

    public async Task<WeatherForecast?> GetByIdAsync(Guid id)
    {
        return await _context.WeatherForecasts.FindAsync(id);
    }

    public async Task<IEnumerable<WeatherForecast>> GetByCityAsync(string city)
    {
        return await _context.WeatherForecasts
            .Where(w => w.City.ToLower() == city.ToLower())
            .ToListAsync();
    }

    public async Task<WeatherForecast> AddAsync(WeatherForecast forecast)
    {
        _context.WeatherForecasts.Add(forecast);
        await _context.SaveChangesAsync();
        return forecast;
    }

    public async Task<WeatherForecast?> UpdateAsync(WeatherForecast forecast)
    {
        _context.WeatherForecasts.Update(forecast);
        await _context.SaveChangesAsync();
        return forecast;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var forecast = await _context.WeatherForecasts.FindAsync(id);
        if (forecast is null)
            return false;

        _context.WeatherForecasts.Remove(forecast);
        await _context.SaveChangesAsync();
        return true;
    }
}
