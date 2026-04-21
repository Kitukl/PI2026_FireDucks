using MediatR;
using Application.Models;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Queries;

public class GetTaskBoardViewTaskPageQuery : IRequest<TaskBoardViewTaskPageDataDto>
{
    public Guid? CurrentUserId { get; set; }
    public string? TaskCode { get; set; }
}

public class GetTaskBoardViewTaskPageQueryHandler : IRequestHandler<GetTaskBoardViewTaskPageQuery, TaskBoardViewTaskPageDataDto>
{
    private readonly ISender _sender;

    public GetTaskBoardViewTaskPageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardViewTaskPageDataDto> Handle(GetTaskBoardViewTaskPageQuery request, CancellationToken cancellationToken)
    {
        var boardModel = await _sender.Send(new GetTaskBoardPageQuery
        {
            CurrentUserId = request.CurrentUserId
        }, cancellationToken);

        TaskBoardTaskCardViewModel? selectedTask = null;
        var comments = new List<StudyHub.Domain.Entities.Comment>();

        var currentUserFullName = string.Empty;
        if (request.CurrentUserId.HasValue)
        {
            var currentUserDto = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
            currentUserFullName = $"{currentUserDto.Name} {currentUserDto.Surname}".Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.TaskCode))
        {
            if (Guid.TryParse(request.TaskCode, out var taskId))
            {
                selectedTask = boardModel.Tasks.FirstOrDefault(task => task.Id == taskId);
            }
            else
            {
                selectedTask = boardModel.Tasks.FirstOrDefault(task =>
                    string.Equals(task.TaskCode, request.TaskCode, StringComparison.OrdinalIgnoreCase));
            }
        }

        selectedTask ??= boardModel.Tasks.FirstOrDefault();

        if (selectedTask != null && selectedTask.Id != Guid.Empty)
        {
            comments = await _sender.Send(new GetCommentsQuery
            {
                TaskId = selectedTask.Id
            }, cancellationToken);
        }

        return new TaskBoardViewTaskPageDataDto
        {
            Board = boardModel,
            SelectedTask = selectedTask,
            Comments = comments,
            CurrentUserFullName = currentUserFullName
        };
    }
}
