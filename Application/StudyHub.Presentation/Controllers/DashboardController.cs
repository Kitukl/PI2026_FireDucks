using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Users.Commands;

namespace Application.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(
        IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dashboardModel = await _mediator.Send(new GetDashBoardQuery
        {
            UserId = Guid.Parse(userId)
        });
        return View("~/Views/UserPlatform/Dashboard/Index.cshtml", dashboardModel);
    }
}
