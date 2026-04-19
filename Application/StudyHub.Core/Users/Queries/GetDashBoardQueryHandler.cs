using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyHub.Core.Common;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class GetDashBoardQuery : IRequest<DashboardViewModel>
{
    public Guid UserId { get; set; }
}

public class GetDashBoardQueryHandler : IRequestHandler<GetDashBoardQuery, DashboardViewModel>
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetDashBoardQueryHandler> _logger;

    public GetDashBoardQueryHandler(IUserRepository userRepository, ITaskRepository taskRepository, ILogger<GetDashBoardQueryHandler> logger)
    {
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _logger = logger;
    }
    public async Task<DashboardViewModel> Handle(GetDashBoardQuery request, CancellationToken cancellationToken)
    {
        var fullName = "Гість";
        
        var user = await _userRepository.GetUserById(request.UserId);
        if (user is not null)
        {
            fullName = $"{user.Name} {user.Surname}".Trim();
        }
        
        else
        {
            _logger.LogInformation("Anonymous user opened the Index page");
        }
        var tasks = await _taskRepository.GetTasksAsync();
        tasks = tasks.Where(task => DashboradHelper.IsVisibleForUser(task, user.Id, user.Group?.Name)).ToList();
        
        var boardModel = await DashboradHelper.BuildTaskBoardModelAsync(user, tasks);
        var quickTasks = boardModel.Tasks
            .Where(task => task.Status is not Status.Done and not Status.Resolved)
            .OrderBy(task => task.Deadline)
            .Take(3)
            .ToList();

        return new DashboardViewModel
        {
            FullName = fullName,
            QuickTasks = quickTasks
        };
    }
}