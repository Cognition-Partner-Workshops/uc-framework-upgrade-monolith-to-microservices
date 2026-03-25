namespace WeatherApi.Application.DTOs;

public class CreateWeatherForecastDto
{
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public string Summary { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeedKmh { get; set; }
}
