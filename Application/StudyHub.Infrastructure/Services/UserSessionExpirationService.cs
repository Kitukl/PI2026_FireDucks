using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyHub.Core.UserSessions.Interfaces;

namespace StudyHub.Infrastructure.Services;

public class UserSessionExpirationService(
    IServiceProvider serviceProvider,
    IOptions<SessionTrackingOptions> options,
    ILogger<UserSessionExpirationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
                var inactivityTimeoutSeconds = Math.Max(30, options.Value.InactivityTimeoutSeconds);
                var inactiveSinceUtc = DateTime.UtcNow.AddSeconds(-inactivityTimeoutSeconds);

                await repository.CloseInactiveSessionsAsync(inactiveSinceUtc, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to close inactive user sessions.");
            }

            var checkIntervalSeconds = Math.Max(10, options.Value.ExpirationCheckIntervalSeconds);
            await Task.Delay(TimeSpan.FromSeconds(checkIntervalSeconds), stoppingToken);
        }
    }
}
