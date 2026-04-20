using Application.Helpers;
using MediatR;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Commands;

public class AddTaskBoardCommentCommand : IRequest<TaskBoardCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid TaskId { get; set; }
    public string? Description { get; set; }
}

public class AddTaskBoardCommentCommandHandler : IRequestHandler<AddTaskBoardCommentCommand, TaskBoardCommandResult>
{
    private readonly ISender _sender;

    public AddTaskBoardCommentCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCommandResult> Handle(AddTaskBoardCommentCommand request, CancellationToken cancellationToken)
    {
        var normalizedDescription = TaskFormattingHelper.PrepareSummary(request.Description, TaskBoardRules.CommentMaxLength);
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            return new TaskBoardCommandResult { IsNoOp = true };
        }

        if (!request.CurrentUserId.HasValue)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var task = await _sender.Send(new GetTaskQuery { Id = request.TaskId }, cancellationToken);
        var currentUserDto = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
        var taskIsVisible = await _sender.Send(new IsTaskVisibleForUserQuery
        {
            Task = task,
            UserId = request.CurrentUserId.Value,
            GroupName = currentUserDto.GroupName
        }, cancellationToken);

        if (!taskIsVisible)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        var userName = $"{currentUserDto.Name} {currentUserDto.Surname}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
        {
            userName = "Guest";
        }

        if (userName.Length > TaskBoardRules.CommentUserNameMaxLength)
        {
            userName = userName[..TaskBoardRules.CommentUserNameMaxLength];
        }

        await _sender.Send(new CreateCommentCommand
        {
            TaskId = request.TaskId,
            UserName = userName,
            Description = normalizedDescription
        }, cancellationToken);

        return new TaskBoardCommandResult { IsSuccess = true };
    }
}
