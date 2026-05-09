using StudyHub.Domain.Entities;

namespace StudyHub.Core.Statistics.Interfaces;

public interface IStatisticRepository
{
    Task<Statistic?> GetRecentStatisticAsync();
    Task<Dictionary<int,double>> GetYearlyActivityAsync(int year);
    Task<Dictionary<int, double>> GetYearlyActivityAsync(Guid userId, int year, CancellationToken cancellationToken);
    Task<(int UserFilesCount, int GroupFilesCount)> GetStorageFileCountsAsync(CancellationToken cancellationToken = default);
    Task<(int StudentsCount, int GroupsCount, int LeadersCount)> GetSystemEntityCountsAsync(CancellationToken cancellationToken = default);
    Task<int> DeleteMonthlyActivityStatisticsAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<int> AddMonthlyActivityStatisticsAsync(int year, int month, IReadOnlyDictionary<Guid, double> averageDayDurationMinutesByUser, CancellationToken cancellationToken = default);
}
