namespace WeatherApi.Application.DTOs;

public class LossReportDto
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }
}
