using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class StatisticRepository(
    SDbContext context,
    IUserRepository userRepository,
    IBlobService blobService) : IStatisticRepository
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

    public async Task<int> GetStorageFileCountAsync(CancellationToken cancellationToken = default)
    {
        var users = (await userRepository.GetUsersAsync()).ToList();
        var count = 0;

        foreach (var user in users)
        {
            var personalContainer = BuildUserStorageContainerName(user.Id);
            var personalFiles = await blobService.ListFilesAsync(personalContainer, cancellationToken);
            count += personalFiles.Count;
        }

        var groupNames = users
            .Select(user => user.Group?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var groupName in groupNames)
        {
            var groupContainer = BuildGroupStorageContainerName(groupName);
            var groupFiles = await blobService.ListFilesAsync(groupContainer, cancellationToken);
            count += groupFiles.Count;
        }

        return count;
    }

    private static string BuildUserStorageContainerName(Guid userId)
    {
        return $"user-storage-{userId:D}";
    }

    private static string BuildGroupStorageContainerName(string groupName)
    {
        return $"group-storage-{groupName}";
    }
}