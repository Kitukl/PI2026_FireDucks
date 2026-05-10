using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.UserSessions.Interfaces;

namespace StudyHub.Infrastructure.Services;

public class MonthlyStatisticsAggregationService(
    IServiceProvider serviceProvider,
    IOptions<SessionTrackingOptions> options,
    ILogger<MonthlyStatisticsAggregationService> logger) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AggregatePreviousMonthAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to aggregate monthly user activity statistics.");
            }
        }
    }

    private async Task AggregatePreviousMonthAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SDbContext>();
        var statisticRepository = scope.ServiceProvider.GetRequiredService<IStatisticRepository>();
        var userSessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var nowUtc = DateTime.UtcNow;
        var currentMonthStartUtc = new DateTime(
            nowUtc.Year,
            nowUtc.Month,
            1,
            0, 0, 0,
            DateTimeKind.Utc);

        if (nowUtc.Day != 1)
        {
            return;
        }

        var targetMonth = currentMonthStartUtc.AddMonths(-1);

        var averageDayDurationMinutesByUser = await userSessionRepository.GetAverageDayDurationMinutesPerUserAsync(
            targetMonth.Year,
            targetMonth.Month,
            cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        await statisticRepository.DeleteMonthlyActivityStatisticsAsync(
            targetMonth.Year,
            targetMonth.Month,
            cancellationToken);

        await statisticRepository.AddMonthlyActivityStatisticsAsync(
            targetMonth.Year,
            targetMonth.Month,
            averageDayDurationMinutesByUser,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        
        var retentionMonths = Math.Max(1, options.Value.RetentionMonths);
        var cutoffUtc =  currentMonthStartUtc.AddMonths(-retentionMonths);
        await userSessionRepository.DeleteClosedSessionsOlderThanAsync(cutoffUtc, cancellationToken);
    }
}
