using StudyHub.Domain.Entities;

namespace StudyHub.Core.UserSessions.Interfaces;

public interface IUserSessionRepository
{
    Task<UserSession> StartSessionAsync(Guid userId, DateTime startedAtUtc, CancellationToken cancellationToken = default);
    Task<bool> TouchSessionAsync(Guid sessionId, Guid userId, DateTime seenAtUtc, CancellationToken cancellationToken = default);
    Task<bool> CloseSessionAsync(Guid sessionId, Guid userId, DateTime closedAtUtc, CancellationToken cancellationToken = default);
    Task<int> CloseInactiveSessionsAsync(DateTime inactiveSinceUtc, CancellationToken cancellationToken = default);
    Task<double> GetAverageSessionDurationMinutesAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<int> DeleteClosedSessionsOlderThanAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default);
}
