using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StudyHub.Core.UserSessions.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Services;

public class UserSessionTrackingService(
    IUserSessionCookieStore userSessionCookieStore,
    IUserSessionRepository userSessionRepository,
    UserManager<User> userManager,
    ILogger<UserSessionTrackingService> logger) : IUserSessionTrackingService
{
    public async Task EnsureSessionStartedAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            return;
        }

        var sessionId = userSessionCookieStore.GetCurrentSessionId();
        if (sessionId.HasValue)
        {
            var updated = await userSessionRepository.TouchSessionAsync(sessionId.Value, userId.Value, DateTime.UtcNow, cancellationToken);
            if (updated)
            {
                return;
            }
        }

        var session = await userSessionRepository.StartSessionAsync(userId.Value, DateTime.UtcNow, cancellationToken);
        userSessionCookieStore.SetCurrentSessionId(session.Id);
    }

    public async Task<bool> HeartbeatAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            return false;
        }

        var sessionId = userSessionCookieStore.GetCurrentSessionId();
        if (sessionId.HasValue)
        {
            var touched = await userSessionRepository.TouchSessionAsync(sessionId.Value, userId.Value, DateTime.UtcNow, cancellationToken);
            if (touched)
            {
                return true;
            }
        }

        var session = await userSessionRepository.StartSessionAsync(userId.Value, DateTime.UtcNow, cancellationToken);
        userSessionCookieStore.SetCurrentSessionId(session.Id);
        return true;
    }

    public async Task<bool> CloseCurrentSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            userSessionCookieStore.ClearCurrentSessionId();
            return false;
        }

        var closed = false;
        var sessionId = userSessionCookieStore.GetCurrentSessionId();
        if (sessionId.HasValue)
        {
            closed = await userSessionRepository.CloseSessionAsync(sessionId.Value, userId.Value, DateTime.UtcNow, cancellationToken);
        }

        userSessionCookieStore.ClearCurrentSessionId();
        return closed;
    }

    private async Task<Guid?> ResolveUserIdAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user != null)
        {
            return user.Id;
        }

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var parsedUserId))
        {
            var exists = await userManager.FindByIdAsync(parsedUserId.ToString("D"));
            if (exists != null)
            {
                return parsedUserId;
            }
        }

        logger.LogWarning("Skipping session tracking because the authenticated principal could not be resolved to an application user.");
        return null;
    }
}
