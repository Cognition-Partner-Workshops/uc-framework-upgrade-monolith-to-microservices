using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Common.Interfaces;

public interface ILossReportService
{
    Task<IEnumerable<LossReportDto>> GetAllAsync();
    Task<LossReportDto?> GetByIdAsync(int id);
    Task<LossReportDto> CreateAsync(CreateLossReportDto dto);
    Task<LossReportDto?> UpdateAsync(int id, UpdateLossReportDto dto);
    Task<bool> DeleteAsync(int id);
}
