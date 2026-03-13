using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Queries;

namespace Application.Controllers;

public class AdminController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        var viewModel = await mediator.Send(new GetUsersStatisticQuery());
        
        return View(viewModel);
    }
}