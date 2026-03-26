using WeatherApi.Domain.Entities;

namespace WeatherApi.Domain.Interfaces;

public interface ILossReportRepository
{
    Task<IEnumerable<LossReport>> GetAllAsync();
    Task<LossReport?> GetByIdAsync(int id);
    Task<LossReport> CreateAsync(LossReport lossReport);
    Task<LossReport?> UpdateAsync(int id, LossReport lossReport);
    Task<bool> DeleteAsync(int id);
}
