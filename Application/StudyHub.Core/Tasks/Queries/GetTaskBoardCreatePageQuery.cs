using Application.Helpers;
using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Queries;

namespace StudyHub.Core.Tasks.Queries;

public class GetTaskBoardCreatePageQuery : IRequest<TaskBoardCreatePageDataDto>
{
    public Guid? CurrentUserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? SubjectId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? IsGroupTask { get; set; }
}

public class GetTaskBoardCreatePageQueryHandler : IRequestHandler<GetTaskBoardCreatePageQuery, TaskBoardCreatePageDataDto>
{
    private readonly ISender _sender;

    public GetTaskBoardCreatePageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardCreatePageDataDto> Handle(GetTaskBoardCreatePageQuery request, CancellationToken cancellationToken)
    {
        var boardModel = await _sender.Send(new GetTaskBoardPageQuery
        {
            CurrentUserId = request.CurrentUserId
        }, cancellationToken);

        var subjects = await _sender.Send(new GetAllSubjectsRequest(), cancellationToken);
        var maxDueDate = DateTime.Today.AddYears(5);

        var dueDate = request.DueDate ?? DateTime.Today.AddDays(1);
        if (dueDate.Date > maxDueDate)
        {
            dueDate = maxDueDate;
        }

        return new TaskBoardCreatePageDataDto
        {
            Board = boardModel,
            Subjects = subjects
                .OrderBy(subject => subject.Name)
                .Select(subject => new SubjectOptionDto
                {
                    Id = subject.Id,
                    Name = subject.Name ?? string.Empty,
                    DisplayName = TaskFormattingHelper.ShortenTextWithEllipsis(subject.Name, TaskBoardRules.SubjectDisplayMaxLength)
                })
                .ToList(),
            Title = request.Title ?? string.Empty,
            Description = request.Description ?? string.Empty,
            SubjectId = request.SubjectId,
            DueDate = dueDate,
            IsGroupTask = request.IsGroupTask ?? true
        };
    }
}
