using Microsoft.EntityFrameworkCore;
using StudyHub.Core.UserSessions.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class UserSessionRepository(SDbContext context) : IUserSessionRepository
{
    public async Task<UserSession> StartSessionAsync(Guid userId, DateTime startedAtUtc, CancellationToken cancellationToken = default)
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EntryTimeUtc = startedAtUtc,
            LastSeenUtc = startedAtUtc,
            IsClosed = false,
            DurationSeconds = 0
        };

        await context.UserSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<bool> TouchSessionAsync(Guid sessionId, Guid userId, DateTime seenAtUtc, CancellationToken cancellationToken = default)
    {
        var session = await context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId && !x.IsClosed, cancellationToken);

        if (session == null)
        {
            return false;
        }

        if (seenAtUtc > session.LastSeenUtc)
        {
            session.LastSeenUtc = seenAtUtc;
            await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<bool> CloseSessionAsync(Guid sessionId, Guid userId, DateTime closedAtUtc, CancellationToken cancellationToken = default)
    {
        var session = await context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId && !x.IsClosed, cancellationToken);

        if (session == null)
        {
            return false;
        }

        var effectiveExitUtc = closedAtUtc < session.EntryTimeUtc
            ? session.EntryTimeUtc
            : closedAtUtc;

        session.LastSeenUtc = effectiveExitUtc > session.LastSeenUtc ? effectiveExitUtc : session.LastSeenUtc;
        session.ExitTimeUtc = effectiveExitUtc;
        session.IsClosed = true;
        session.DurationSeconds = (int)Math.Max(0, Math.Round((effectiveExitUtc - session.EntryTimeUtc).TotalSeconds));

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> CloseInactiveSessionsAsync(DateTime inactiveSinceUtc, CancellationToken cancellationToken = default)
    {
        var sessions = await context.UserSessions
            .Where(session => !session.IsClosed && session.LastSeenUtc < inactiveSinceUtc)
            .ToListAsync(cancellationToken);

        if (sessions.Count == 0)
        {
            return 0;
        }

        foreach (var session in sessions)
        {
            session.ExitTimeUtc = session.LastSeenUtc;
            session.IsClosed = true;
            session.DurationSeconds = (int)Math.Max(0, Math.Round((session.LastSeenUtc - session.EntryTimeUtc).TotalSeconds));
        }

        await context.SaveChangesAsync(cancellationToken);
        return sessions.Count;
    }

    public async Task<Dictionary<Guid, double>> GetAverageDayDurationMinutesPerUserAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var dailyTotalsQuery = context.UserSessions
            .AsNoTracking()
            .Where(session => session.IsClosed &&
                              session.ExitTimeUtc.HasValue &&
                              session.ExitTimeUtc.Value.Year == year &&
                              session.ExitTimeUtc.Value.Month == month)
            .GroupBy(session => new
            {
                session.UserId,
                Day = session.ExitTimeUtc!.Value.Date
            })
            .Select(group => new
            {
                group.Key.UserId,
                DailyTotalSeconds = group.Sum(session => session.DurationSeconds)
            });

        return await dailyTotalsQuery
            .GroupBy(item => item.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                AverageDayDurationMinutes = Math.Round(
                    group.Average(item => (double)item.DailyTotalSeconds) / 60d,
                    2,
                    MidpointRounding.AwayFromZero)
            })
            .ToDictionaryAsync(item => item.UserId, item => item.AverageDayDurationMinutes, cancellationToken);
    }

    public Task<int> DeleteClosedSessionsOlderThanAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default)
    {
        return context.UserSessions
            .Where(session => session.IsClosed &&
                              session.ExitTimeUtc.HasValue &&
                              session.ExitTimeUtc.Value < cutoffUtc)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
