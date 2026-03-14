using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class StatisticRepository(SDbContext context) : IStatisticRepository
{
    public Task<Statistic?> GetRecentStatisticAsync()
    {
        var recentStats = context.Statistics.OrderByDescending(a => a.CreatedAt).FirstOrDefaultAsync();
        
        return recentStats;
    }

    public async Task<Dictionary<int, double>> GetYearlyActivityAsync(int year)
    {
        return await context.Statistics
            .Where(s => s.CreatedAt.Year == year)
            .GroupBy(s => s.CreatedAt.Month)
            .AsNoTracking()
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Sum(s => s.UserActivityPerMonth));
    }
}