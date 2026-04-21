using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.UserSessions.Commands;

namespace Application.Controllers;

[Authorize]
[Route("session")]
public class SessionController(ISender sender) : Controller
{
    [HttpPost("heartbeat")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Heartbeat(CancellationToken cancellationToken)
    {
        await sender.Send(new HeartbeatUserSessionCommand(User), cancellationToken);

        return Ok();
    }

    [HttpPost("close")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Close(CancellationToken cancellationToken)
    {
        await sender.Send(new CloseUserSessionCommand(User), cancellationToken);

        return Ok();
    }
}
