using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Application.DTOs;

public class CreateLossReportDto
{
    [Required(ErrorMessage = "Policy number is required.")]
    [MaxLength(50, ErrorMessage = "Policy number cannot exceed 50 characters.")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}
