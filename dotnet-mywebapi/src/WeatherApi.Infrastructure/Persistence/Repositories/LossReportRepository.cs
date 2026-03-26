using Microsoft.EntityFrameworkCore;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Infrastructure.Persistence.Repositories;

public class LossReportRepository : ILossReportRepository
{
    private readonly AppDbContext _context;

    public LossReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LossReport>> GetAllAsync()
    {
        return await _context.LossReports
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<LossReport?> GetByIdAsync(int id)
    {
        return await _context.LossReports.FindAsync(id);
    }

    public async Task<LossReport> CreateAsync(LossReport lossReport)
    {
        _context.LossReports.Add(lossReport);
        await _context.SaveChangesAsync();
        return lossReport;
    }

    public async Task<LossReport?> UpdateAsync(int id, LossReport lossReport)
    {
        var existing = await _context.LossReports.FindAsync(id);
        if (existing == null)
            return null;

        existing.PolicyNumber = lossReport.PolicyNumber;
        existing.Description = lossReport.Description;
        existing.Amount = lossReport.Amount;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.LossReports.FindAsync(id);
        if (existing == null)
            return false;

        _context.LossReports.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
