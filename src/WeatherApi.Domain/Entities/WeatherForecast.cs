namespace WeatherApi.Domain.Entities;

public class WeatherForecast
{
    public Guid Id { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public double TemperatureFahrenheit => (TemperatureCelsius * 9.0 / 5.0) + 32;
    public string Summary { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeedKmh { get; set; }
}
