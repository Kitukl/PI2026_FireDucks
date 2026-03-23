using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;

namespace Application.Controllers;

public class AdminController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var user = await mediator.Send(new GetUsersStatisticRequest());
        var tasksCount = await mediator.Send(new GetTaskCountRequest());
        var taskStatusCount = await mediator.Send(new GetGroupedTaskStatsRequest());
        
        var viewModel = new SystemStatisticViewModel()
        {
            CreatedAt = user.CreatedAt,
            UserActivityPerMonth = user.UserActivityPerMonth,
            GropedTaskCount = taskStatusCount,
            FileCount = user.FileCount,
            TaskCount = tasksCount
        };
            
        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        var users = await mediator.Send(new GetUsersRequest());
        
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await mediator.Send(new GetUserRequest(id));
        
        return View(user);
    }

    [HttpGet("/Admin/Requests")]
    public IActionResult Requests()
    {
        return View();
    }

    [HttpGet("/Admin/Requests/View/{feedbackId?}")]
    public IActionResult RequestView(string? feedbackId)
    {
        ViewBag.FeedbackId = string.IsNullOrWhiteSpace(feedbackId) ? "1" : feedbackId;
        ViewBag.OpenRequestModal = true;
        return View("Requests");
    }
}