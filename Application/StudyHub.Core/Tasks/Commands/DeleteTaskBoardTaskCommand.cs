using MediatR;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Commands;

public class DeleteTaskBoardTaskCommand : IRequest<TaskBoardCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid TaskId { get; set; }
}

public class DeleteTaskBoardTaskCommandHandler : IRequestHandler<DeleteTaskBoardTaskCommand, TaskBoardCommandResult>
{
    private readonly ISender _sender;

    public DeleteTaskBoardTaskCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCommandResult> Handle(DeleteTaskBoardTaskCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var currentUser = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
        var taskToDelete = await _sender.Send(new GetTaskQuery { Id = request.TaskId }, cancellationToken);
        var taskIsVisible = await _sender.Send(new IsTaskVisibleForUserQuery
        {
            Task = taskToDelete,
            UserId = request.CurrentUserId.Value,
            GroupName = currentUser.GroupName
        }, cancellationToken);

        if (!taskIsVisible)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        await _sender.Send(new DeleteCommand
        {
            Id = request.TaskId
        }, cancellationToken);

        return new TaskBoardCommandResult { IsSuccess = true };
    }
}
