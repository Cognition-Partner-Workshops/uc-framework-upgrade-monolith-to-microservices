namespace WeatherApi.Domain.Entities;

public class WeatherForecast
{
    public Guid Id { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public double TemperatureFahrenheit => 32 + (TemperatureCelsius / 0.5556);
    public string Summary { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeedKmh { get; set; }
}
