using MediatR;
using System.Security.Claims;
using StudyHub.Core.UserSessions.Interfaces;

namespace StudyHub.Core.UserSessions.Commands;

public record EnsureUserSessionCommand(ClaimsPrincipal Principal) : IRequest;

public class EnsureUserSessionCommandHandler(IUserSessionTrackingService userSessionTrackingService)
    : IRequestHandler<EnsureUserSessionCommand>
{
    public async Task Handle(EnsureUserSessionCommand request, CancellationToken cancellationToken)
    {
        await userSessionTrackingService.EnsureSessionStartedAsync(request.Principal, cancellationToken);
    }
}
