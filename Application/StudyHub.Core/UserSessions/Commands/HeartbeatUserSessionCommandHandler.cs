using MediatR;
using System.Security.Claims;
using StudyHub.Core.UserSessions.Interfaces;

namespace StudyHub.Core.UserSessions.Commands;

public record HeartbeatUserSessionCommand(ClaimsPrincipal Principal) : IRequest<bool>;

public class HeartbeatUserSessionCommandHandler(IUserSessionTrackingService userSessionTrackingService)
    : IRequestHandler<HeartbeatUserSessionCommand, bool>
{
    public Task<bool> Handle(HeartbeatUserSessionCommand request, CancellationToken cancellationToken)
    {
        return userSessionTrackingService.HeartbeatAsync(request.Principal, cancellationToken);
    }
}
