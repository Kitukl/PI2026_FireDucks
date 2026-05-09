namespace StudyHub.Core.UserSessions.Interfaces;

public interface IUserSessionCookieStore
{
    Guid? GetCurrentSessionId();
    void SetCurrentSessionId(Guid sessionId);
    void ClearCurrentSessionId();
}
