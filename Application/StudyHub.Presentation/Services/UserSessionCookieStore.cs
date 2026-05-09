using Microsoft.AspNetCore.Http;
using StudyHub.Core.UserSessions.Interfaces;

namespace Application.Services;

public class UserSessionCookieStore(IHttpContextAccessor httpContextAccessor) : IUserSessionCookieStore
{
    private const string SessionCookieName = "studyhub-session-id";

    public Guid? GetCurrentSessionId()
    {
        var rawValue = httpContextAccessor.HttpContext?.Request.Cookies[SessionCookieName];
        return Guid.TryParse(rawValue, out var sessionId) ? sessionId : null;
    }

    public void SetCurrentSessionId(Guid sessionId)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        httpContext.Response.Cookies.Append(SessionCookieName, sessionId.ToString("D"), new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = httpContext.Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    public void ClearCurrentSessionId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        httpContext.Response.Cookies.Delete(SessionCookieName, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = httpContext.Request.IsHttps
        });
    }
}
