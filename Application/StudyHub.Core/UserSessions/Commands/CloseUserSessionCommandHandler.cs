using MediatR;
using System.Security.Claims;
using StudyHub.Core.UserSessions.Interfaces;

namespace StudyHub.Core.UserSessions.Commands;

public record CloseUserSessionCommand(ClaimsPrincipal Principal) : IRequest<bool>;

public class CloseUserSessionCommandHandler(IUserSessionTrackingService userSessionTrackingService)
    : IRequestHandler<CloseUserSessionCommand, bool>
{
    public Task<bool> Handle(CloseUserSessionCommand request, CancellationToken cancellationToken)
    {
        return userSessionTrackingService.CloseCurrentSessionAsync(request.Principal, cancellationToken);
    }
}
