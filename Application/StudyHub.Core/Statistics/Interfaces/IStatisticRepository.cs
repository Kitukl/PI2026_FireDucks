using StudyHub.Domain.Entities;

namespace StudyHub.Core.Statistics.Interfaces;

public interface IStatisticRepository
{
    Task<Statistic?> GetRecentStatisticAsync();
    Task<Dictionary<int,double>> GetYearlyActivityAsync(int year);
    Task<(int UserFilesCount, int GroupFilesCount)> GetStorageFileCountsAsync(CancellationToken cancellationToken = default);
    Task<(int StudentsCount, int GroupsCount, int LeadersCount)> GetSystemEntityCountsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasStatisticForMonthAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<Statistic> AddMonthlyActivityStatisticAsync(int year, int month, double averageSessionDurationMinutes, CancellationToken cancellationToken = default);
}
