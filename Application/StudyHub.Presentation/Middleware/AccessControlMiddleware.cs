using StudyHub.Domain.Enums;

namespace Application.Middleware;

public class AccessControlMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        var pathValue = path.Value ?? string.Empty;

        var isStaticAsset = Path.HasExtension(pathValue);
        var isSessionTrackingPath = IsPath(pathValue, "/session");
        var isLoginPath = IsPath(pathValue, "/login");
        var isAuthTechnicalPath = IsPath(pathValue, "/user/login-microsoft") ||
                                  IsPath(pathValue, "/user/callback") ||
                                  IsPath(pathValue, "/signin-microsoft") ||
                                  IsPath(pathValue, "/user/access-denied");
        var isBypassedPath = isLoginPath || isAuthTechnicalPath || isSessionTrackingPath || isStaticAsset;

        if (!isBypassedPath && context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect("/login");
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var isAdmin = context.User.IsInRole(nameof(Role.Admin));
            var isStudent = context.User.IsInRole(nameof(Role.Student));
            var isLeader = context.User.IsInRole(nameof(Role.Leader));
            var isAdminPath = IsPath(pathValue, "/Admin");
            var isLogoutPath = IsPath(pathValue, "/user/logout");

            if (!isAdmin && isAdminPath)
            {
                context.Response.Redirect("/Home/Index");
                return;
            }

            if (isAdmin && !isAdminPath && !isLogoutPath && !isAuthTechnicalPath && !isSessionTrackingPath && !isStaticAsset)
            {
                context.Response.Redirect("/Admin/Dashboard");
                return;
            }

            if (isStudent && !isLeader && IsPath(pathValue, "/TaskBoard/ReviewGroup"))
            {
                context.Response.Redirect("/Home/Index");
                return;
            }
        }

        await next(context);
    }

    private static bool IsPath(string? value, string prefix)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}
