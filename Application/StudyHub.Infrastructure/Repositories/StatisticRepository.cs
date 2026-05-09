using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

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
                g => g.Average(s => s.UserActivityPerMonth));
    }

    public async Task<Dictionary<int, double>> GetYearlyActivityAsync(Guid userId, int year,CancellationToken cancellationToken)
    {
        return await context.Statistics
            .Where(s => s.CreatedAt.Year == year && s.Users.Any(u => u.Id == userId))
            .GroupBy(s => s.CreatedAt.Month)
            .AsNoTracking()
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Average(s => s.UserActivityPerMonth),
                cancellationToken);
    }

    public async Task<(int UserFilesCount, int GroupFilesCount)> GetStorageFileCountsAsync(CancellationToken cancellationToken = default)
    {
        var users = (await userRepository.GetUsersAsync()).ToList();
        var userFilesCount = 0;

        foreach (var user in users)
        {
            var personalContainer = BuildUserStorageContainerName(user.Id);
            var personalFiles = await blobService.ListFilesAsync(personalContainer, cancellationToken);
            userFilesCount += personalFiles.Count;
        }

        var groupNames = users
            .Select(user => user.Group?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var groupFilesCount = 0;

        foreach (var groupName in groupNames)
        {
            var groupContainer = BuildGroupStorageContainerName(groupName);
            var groupFiles = await blobService.ListFilesAsync(groupContainer, cancellationToken);
            groupFilesCount += groupFiles.Count;
        }

        return (userFilesCount, groupFilesCount);
    }

    public async Task<(int StudentsCount, int GroupsCount, int LeadersCount)> GetSystemEntityCountsAsync(CancellationToken cancellationToken = default)
    {
        var studentsCount = await GetUsersInRoleCountAsync(Role.Student, cancellationToken);
        var groupsCount = await context.Groups.CountAsync(cancellationToken);
        var leadersCount = await GetUsersInRoleCountAsync(Role.Leader, cancellationToken);

        return (studentsCount, groupsCount, leadersCount);
    }

    public Task<int> DeleteMonthlyActivityStatisticsAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return context.Statistics
            .Where(statistic => statistic.CreatedAt.Year == year && statistic.CreatedAt.Month == month)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> AddMonthlyActivityStatisticsAsync(
        int year,
        int month,
        IReadOnlyDictionary<Guid, double> averageDayDurationMinutesByUser,
        CancellationToken cancellationToken = default)
    {
        if (averageDayDurationMinutesByUser.Count == 0)
        {
            return 0;
        }

        var users = await context.Users
            .Where(user => averageDayDurationMinutesByUser.Keys.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, cancellationToken);

        var createdAtUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var statistics = averageDayDurationMinutesByUser
            .Where(item => users.ContainsKey(item.Key))
            .Select(item => new Statistic
            {
                Id = Guid.NewGuid(),
                CreatedAt = createdAtUtc,
                UserActivityPerMonth = item.Value,
                FilesCount = 0,
                Users = [users[item.Key]],
                Tasks = []
            })
            .ToList();

        if (statistics.Count == 0)
        {
            return 0;
        }

        await context.Statistics.AddRangeAsync(statistics, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return statistics.Count;
    }

    private Task<int> GetUsersInRoleCountAsync(Role role, CancellationToken cancellationToken)
    {
        var normalizedRoleName = role.ToString().ToUpperInvariant();

        return (
            from userRole in context.UserRoles
            join identityRole in context.Roles on userRole.RoleId equals identityRole.Id
            where identityRole.NormalizedName == normalizedRoleName
            select userRole.UserId
        ).Distinct().CountAsync(cancellationToken);
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
