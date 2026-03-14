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

    public async Task<IEnumerable<Statistic>> GetThisYearStatisticsAsync()
    {
        var thisYearStats = await context.Statistics
            .Where(s => s.CreatedAt.Year == DateTime.Now.Year)
            .AsNoTracking()
            .ToListAsync();
        
        return thisYearStats;
    }
}