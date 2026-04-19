using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Users.Commands;

namespace Application.Controllers;

public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(
        IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize]
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dashboardModel = await _mediator.Send(new GetDashBoardQuery
        {
            UserId = Guid.Parse(userId)
        });
        return View("~/Views/Home/Dashboard/Index.cshtml", dashboardModel);
    }
}
