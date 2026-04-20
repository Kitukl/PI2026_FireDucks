using Application.Helpers;
using MediatR;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Commands;

public class UpdateTaskBoardTaskStatusCommand : IRequest<UpdateTaskBoardTaskStatusResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid TaskId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateTaskBoardTaskStatusResult : TaskBoardCommandResult
{
    public string Status { get; set; } = string.Empty;
    public string NormalizedStatus { get; set; } = string.Empty;
}

public class UpdateTaskBoardTaskStatusCommandHandler : IRequestHandler<UpdateTaskBoardTaskStatusCommand, UpdateTaskBoardTaskStatusResult>
{
    private readonly ISender _sender;

    public UpdateTaskBoardTaskStatusCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<UpdateTaskBoardTaskStatusResult> Handle(UpdateTaskBoardTaskStatusCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new UpdateTaskBoardTaskStatusResult { IsNotFound = true, Status = request.Status };
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
            return new UpdateTaskBoardTaskStatusResult { IsForbidden = true, Status = request.Status };
        }

        var parsedStatus = TaskBoardControllerHelper.ParseTaskStatus(request.Status);

        await _sender.Send(new UpdateTaskCommand
        {
            Id = request.TaskId,
            Status = parsedStatus,
            Subject = task.Subject
        }, cancellationToken);

        return new UpdateTaskBoardTaskStatusResult
        {
            IsSuccess = true,
            Status = request.Status,
            NormalizedStatus = TaskBoardControllerHelper.NormalizeTaskStatus(parsedStatus)
        };
    }
}
