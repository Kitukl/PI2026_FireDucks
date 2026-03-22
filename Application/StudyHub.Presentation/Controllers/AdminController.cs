using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Enums;

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
            UserRoleCount = new Dictionary<string, int>(),
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
        var userDto = await mediator.Send(new GetUserRequest(id));
        
        var viewModel = new UpdateUserViewModel
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname ?? "",
            GroupName = userDto.GroupName,
            Roles = userDto.Roles,
            AvailableRoles = Enum.GetNames(typeof(Role)).ToList()
        };
        
        return View("UpdateUser",viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserUpdateDto user)
    {
        await mediator.Send(new UpdateUserCommand 
        { 
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Photo = user.PhotoUrl,
            GroupName = user.GroupName
        });
    
        return RedirectToAction("GetUser", new { id = user.Id });
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await mediator.Send(new DeleteUserCommand { UserId = id });
    
        return RedirectToAction("Users"); 
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUserRole(UserRoleUpdateDto dto)
    {
        await mediator.Send(new AddUserRoleCommand 
        { 
            UserId = dto.Id, 
            Role = dto.Role 
        });
    
        return RedirectToAction("GetUser", new { id = dto.Id });
    }

    [HttpPost] 
    public async Task<IActionResult> RemoveUserRole(UserRoleUpdateDto dto)
    {
        await mediator.Send(new RemoveUserRoleCommand 
        { 
            UserId = dto.Id, 
            Role = dto.Role 
        });
    
        return RedirectToAction("GetUser", new { id = dto.Id });
    }
}