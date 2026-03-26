using WeatherApi.Application.Common.Interfaces;
using WeatherApi.Application.DTOs;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Infrastructure.Services;

public class LossReportService : ILossReportService
{
    private readonly ILossReportRepository _repository;

    public LossReportService(ILossReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LossReportDto>> GetAllAsync()
    {
        var reports = await _repository.GetAllAsync();
        return reports.Select(MapToDto);
    }

    public async Task<LossReportDto?> GetByIdAsync(int id)
    {
        var report = await _repository.GetByIdAsync(id);
        return report == null ? null : MapToDto(report);
    }

    public async Task<LossReportDto> CreateAsync(CreateLossReportDto dto)
    {
        var entity = new LossReport
        {
            PolicyNumber = dto.PolicyNumber,
            Description = dto.Description,
            Amount = dto.Amount,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<LossReportDto?> UpdateAsync(int id, UpdateLossReportDto dto)
    {
        var entity = new LossReport
        {
            PolicyNumber = dto.PolicyNumber,
            Description = dto.Description,
            Amount = dto.Amount
        };

        var updated = await _repository.UpdateAsync(id, entity);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static LossReportDto MapToDto(LossReport entity)
    {
        return new LossReportDto
        {
            Id = entity.Id,
            PolicyNumber = entity.PolicyNumber,
            Description = entity.Description,
            Amount = entity.Amount,
            CreatedDate = entity.CreatedDate
        };
    }
}
