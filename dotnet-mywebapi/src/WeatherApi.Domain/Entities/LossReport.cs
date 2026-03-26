using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Domain.Entities;

public class LossReport
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
