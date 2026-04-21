using MediatR;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Commands;

public class DeleteTaskBoardCommentCommand : IRequest<TaskBoardCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid TaskId { get; set; }
    public Guid CommentId { get; set; }
    public bool IsLeader { get; set; }
}

public class DeleteTaskBoardCommentCommandHandler : IRequestHandler<DeleteTaskBoardCommentCommand, TaskBoardCommandResult>
{
    private readonly ISender _sender;

    public DeleteTaskBoardCommentCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCommandResult> Handle(DeleteTaskBoardCommentCommand request, CancellationToken cancellationToken)
    {
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

        var comments = await _sender.Send(new GetCommentsQuery
        {
            TaskId = request.TaskId
        }, cancellationToken);

        var targetComment = comments.FirstOrDefault(comment => comment.Id == request.CommentId);
        if (targetComment == null)
        {
            return new TaskBoardCommandResult { IsNotFound = true };
        }

        var currentUserFullName = $"{currentUserDto.Name} {currentUserDto.Surname}".Trim();

        var canDeleteComment = request.IsLeader ||
                               (!string.IsNullOrWhiteSpace(currentUserFullName) &&
                                string.Equals(targetComment.UserName, currentUserFullName, StringComparison.OrdinalIgnoreCase));

        if (!canDeleteComment)
        {
            return new TaskBoardCommandResult { IsForbidden = true };
        }

        await _sender.Send(new DeleteCommentCommand
        {
            Id = request.CommentId
        }, cancellationToken);

        return new TaskBoardCommandResult { IsSuccess = true };
    }
}
