using Application.Helpers;
using MediatR;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Tasks.Commands;

public class EditTaskBoardTaskCommand : IRequest<TaskBoardCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid TaskId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ResourceUrl { get; set; }
}

public class EditTaskBoardTaskCommandHandler : IRequestHandler<EditTaskBoardTaskCommand, TaskBoardCommandResult>
{
    private readonly ISender _sender;

    public EditTaskBoardTaskCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCommandResult> Handle(EditTaskBoardTaskCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var task = await _sender.Send(new GetTaskQuery { Id = request.TaskId }, cancellationToken);
        var currentUser = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);

        var taskIsVisible = await _sender.Send(new IsTaskVisibleForUserQuery
        {
            Task = task,
            UserId = request.CurrentUserId.Value,
            GroupName = currentUser.GroupName
        }, cancellationToken);

        if (!taskIsVisible)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var normalizedTitle = (request.Title ?? string.Empty).Trim();
        var normalizedDescription = TaskFormattingHelper.PrepareSummary(request.Description, TaskBoardRules.SummaryMaxLength);

        await _sender.Send(new UpdateTaskFullCommand
        {
            Id = request.TaskId,
            Title = normalizedTitle,
            Description = normalizedDescription,
            ResourceUrl = request.ResourceUrl,
            Status = task.Status,
            Subject = task.Subject
        }, cancellationToken);

        return new TaskBoardCommandResult { IsSuccess = true };
    }
}