using MediatR;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Commands;

public class AddUsersToTaskBoardGroupCommand : IRequest<TaskBoardCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public List<Guid> SelectedUserIds { get; set; } = new();
}

public class AddUsersToTaskBoardGroupCommandHandler : IRequestHandler<AddUsersToTaskBoardGroupCommand, TaskBoardCommandResult>
{
    private readonly ISender _sender;

    public AddUsersToTaskBoardGroupCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCommandResult> Handle(AddUsersToTaskBoardGroupCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var currentUser = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
        var groupName = currentUser.GroupName;

        if (string.IsNullOrWhiteSpace(groupName))
        {
            return new TaskBoardCommandResult { IsNoOp = true };
        }

        foreach (var userId in request.SelectedUserIds.Distinct())
        {
            await _sender.Send(new AddUserToGroupCommand
            {
                UserId = userId,
                GroupName = groupName
            }, cancellationToken);
        }

        return new TaskBoardCommandResult { IsSuccess = true };
    }
}
