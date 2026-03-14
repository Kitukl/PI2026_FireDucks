using StudyHub.Domain.Entities;

namespace StudyHub.Core.Statistics.Interfaces;

public interface IStatisticRepository
{
    Task<Statistic?> GetRecentStatisticAsync();
    Task<Dictionary<int,double>> GetYearlyActivityAsync(int year);
}