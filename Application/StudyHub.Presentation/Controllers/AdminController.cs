using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Statistics.Queries;

namespace Application.Controllers;

public class AdminController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        var viewModel = await mediator.Send(new GetUsersStatisticRequest());
        
        return View(viewModel);
    }
}