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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to aggregate monthly session statistics.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task AggregatePreviousMonthAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var statisticRepository = scope.ServiceProvider.GetRequiredService<IStatisticRepository>();
        var userSessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var nowUtc = DateTime.UtcNow;
        var targetMonth = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1);

        var alreadyExists = await statisticRepository.HasStatisticForMonthAsync(
            targetMonth.Year,
            targetMonth.Month,
            cancellationToken);

        if (!alreadyExists)
        {
            var averageMinutes = await userSessionRepository.GetAverageSessionDurationMinutesAsync(
                targetMonth.Year,
                targetMonth.Month,
                cancellationToken);

            await statisticRepository.AddMonthlyActivityStatisticAsync(
                targetMonth.Year,
                targetMonth.Month,
                averageMinutes,
                cancellationToken);
        }

        var retentionMonths = Math.Max(1, options.Value.RetentionMonths);
        var cutoffUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-retentionMonths);
        await userSessionRepository.DeleteClosedSessionsOlderThanAsync(cutoffUtc, cancellationToken);
    }
}
