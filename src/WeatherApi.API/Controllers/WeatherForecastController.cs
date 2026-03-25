using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _service;

    public WeatherForecastController(IWeatherForecastService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all weather forecasts
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecastDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetAll()
    {
        var forecasts = await _service.GetAllAsync();
        return Ok(forecasts);
    }

    /// <summary>
    /// Get a weather forecast by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherForecastDto>> GetById(Guid id)
    {
        var forecast = await _service.GetByIdAsync(id);
        if (forecast is null)
            return NotFound();

        return Ok(forecast);
    }

    /// <summary>
    /// Get weather forecasts by city name
    /// </summary>
    [HttpGet("city/{city}")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecastDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetByCity(string city)
    {
        var forecasts = await _service.GetByCityAsync(city);
        return Ok(forecasts);
    }

    /// <summary>
    /// Create a new weather forecast
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherForecastDto>> Create([FromBody] CreateWeatherForecastDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing weather forecast
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherForecastDto>> Update(Guid id, [FromBody] UpdateWeatherForecastDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    /// <summary>
    /// Delete a weather forecast
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Generate a random weather forecast and persist it
    /// </summary>
    [HttpPost("random")]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WeatherForecastDto>> GenerateRandom([FromQuery] string? city = null)
    {
        var forecast = await _service.GenerateRandomAsync(city);
        return CreatedAtAction(nameof(GetById), new { id = forecast.Id }, forecast);
    }
}
