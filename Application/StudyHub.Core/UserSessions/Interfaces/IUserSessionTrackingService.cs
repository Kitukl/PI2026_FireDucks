using System.Security.Claims;

namespace StudyHub.Core.UserSessions.Interfaces;

public interface IUserSessionTrackingService
{
    Task EnsureSessionStartedAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<bool> HeartbeatAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<bool> CloseCurrentSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}
