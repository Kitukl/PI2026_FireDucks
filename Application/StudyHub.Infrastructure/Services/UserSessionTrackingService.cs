using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StudyHub.Core.UserSessions.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Services;

public class UserSessionTrackingService(
    IHttpContextAccessor httpContextAccessor,
    IUserSessionRepository userSessionRepository,
    UserManager<User> userManager,
    ILogger<UserSessionTrackingService> logger) : IUserSessionTrackingService
{
    private const string SessionCookieName = "studyhub-session-id";

    public async Task EnsureSessionStartedAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            return;
        }

        if (TryGetSessionId(httpContext.Request.Cookies[SessionCookieName], out var sessionId))
        {
            var updated = await userSessionRepository.TouchSessionAsync(sessionId, userId.Value, DateTime.UtcNow, cancellationToken);
            if (updated)
            {
                return;
            }
        }

        var session = await userSessionRepository.StartSessionAsync(userId.Value, DateTime.UtcNow, cancellationToken);
        AppendSessionCookie(httpContext, session.Id);
    }

    public async Task<bool> HeartbeatAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            return false;
        }

        if (TryGetSessionId(httpContext.Request.Cookies[SessionCookieName], out var sessionId))
        {
            var touched = await userSessionRepository.TouchSessionAsync(sessionId, userId.Value, DateTime.UtcNow, cancellationToken);
            if (touched)
            {
                return true;
            }
        }

        var session = await userSessionRepository.StartSessionAsync(userId.Value, DateTime.UtcNow, cancellationToken);
        AppendSessionCookie(httpContext, session.Id);
        return true;
    }

    public async Task<bool> CloseCurrentSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        var userId = await ResolveUserIdAsync(principal);
        if (userId == null)
        {
            DeleteSessionCookie(httpContext);
            return false;
        }

        var closed = false;
        if (TryGetSessionId(httpContext.Request.Cookies[SessionCookieName], out var sessionId))
        {
            closed = await userSessionRepository.CloseSessionAsync(sessionId, userId.Value, DateTime.UtcNow, cancellationToken);
        }

        DeleteSessionCookie(httpContext);
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

    private static bool TryGetSessionId(string? rawValue, out Guid sessionId)
    {
        return Guid.TryParse(rawValue, out sessionId);
    }

    private static void AppendSessionCookie(HttpContext httpContext, Guid sessionId)
    {
        httpContext.Response.Cookies.Append(SessionCookieName, sessionId.ToString("D"), new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = httpContext.Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private static void DeleteSessionCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(SessionCookieName, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = httpContext.Request.IsHttps
        });
    }
}
