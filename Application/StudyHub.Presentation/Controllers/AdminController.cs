using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;

namespace Application.Controllers;

public class AdminController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var user = await mediator.Send(new GetUsersStatisticRequest());
        var tasksCount = await mediator.Send(new GetTaskCountRequest());
        var userRoleCount = await mediator.Send(new GetUserCountByRole());
        
        var viewModel = new SystemStatisticViewModel()
        {
            CreatedAt = user.CreatedAt,
            UserActivityPerMonth = user.UserActivityPerMonth,
            UserRoleCount =  userRoleCount,
            FileCount = user.FileCount,
            TaskCount = tasksCount,
        };
            
        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        var users = await mediator.Send(new GetUsersRequest());
        
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateUser(Guid id)
    {
        var user = await mediator.Send(new GetUserRequest(id));
        
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserUpdateDto user)
    {
        await mediator.Send(new UpdateUserCommand(user));
        
        return RedirectToAction("UpdateUser");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await mediator.Send(new DeleteUserCommand(id));
        
        return RedirectToAction("GetUsers");
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUserRole(UserRoleUpdateDto user)
    {
        await mediator.Send(new AddUserRoleCommand(user));
        
        return RedirectToAction("UpdateUser", new { id = user.Id });
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveUserRole(UserRoleUpdateDto user)
    {
        await mediator.Send(new RemoveUserRoleCommand(user));
        
        return RedirectToAction("UpdateUser", new { id = user.Id });
    }
}