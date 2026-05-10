using Application.Helpers;
using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Tasks.Commands;

public class CreateTaskBoardTaskCommand : IRequest<CreateTaskBoardTaskResult>
{
    public Guid? CurrentUserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? SubjectId { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsGroupTask { get; set; }
    public string? ResourceUrl { get; set; }
}

public class CreateTaskBoardTaskResult
{
    public bool IsSuccess { get; set; }
    public bool IsForbidden { get; set; }
    public Guid? CreatedTaskId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? SubjectId { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsGroupTask { get; set; }
    public string? ResourceUrl { get; set; }

    public List<CreateTaskBoardTaskValidationError> Errors { get; set; } = new();
    public TaskBoardCreatePageDataDto? InvalidPageData { get; set; }

    public static CreateTaskBoardTaskResult Forbidden() => new() { IsForbidden = true };
}

public class CreateTaskBoardTaskValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class CreateTaskBoardTaskCommandHandler : IRequestHandler<CreateTaskBoardTaskCommand, CreateTaskBoardTaskResult>
{
    private readonly ISender _sender;

    public CreateTaskBoardTaskCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<CreateTaskBoardTaskResult> Handle(CreateTaskBoardTaskCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return CreateTaskBoardTaskResult.Forbidden();
        }

        var normalizedTitle = (request.Title ?? string.Empty).Trim();
        var normalizedDescription = TaskFormattingHelper.PrepareSummary(request.Description, TaskBoardRules.SummaryMaxLength);

        var result = new CreateTaskBoardTaskResult
        {
            Title = normalizedTitle,
            Description = normalizedDescription,
            SubjectId = request.SubjectId,
            DueDate = request.DueDate,
            IsGroupTask = request.IsGroupTask
        };

        if (request.SubjectId == null || request.SubjectId == Guid.Empty)
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "SubjectId",
                Message = "Subject is required."
            });
        }

        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "Title",
                Message = "Task title is required."
            });
        }

        if (normalizedTitle.Length > TaskBoardRules.TaskTitleMaxLength)
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "Title",
                Message = $"Task title cannot exceed {TaskBoardRules.TaskTitleMaxLength} characters."
            });
        }

        if (normalizedDescription.Length > TaskBoardRules.SummaryMaxLength)
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "Description",
                Message = $"Summary cannot exceed {TaskBoardRules.SummaryMaxLength} characters."
            });
        }

        var maxDueDate = TaskBoardRules.GetMaxDueDate();
        if (request.DueDate.Date > maxDueDate)
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "DueDate",
                Message = "Due date cannot exceed 5 years from today."
            });
        }

        var selectedSubject = request.SubjectId == null
            ? null
            : await _sender.Send(new GetSubjectByIdRequest(request.SubjectId.Value), cancellationToken);

        if (selectedSubject == null)
        {
            result.Errors.Add(new CreateTaskBoardTaskValidationError
            {
                Field = "SubjectId",
                Message = "Selected subject was not found."
            });
        }

        if (result.Errors.Count > 0)
        {
            result.InvalidPageData = await _sender.Send(new GetTaskBoardCreatePageQuery
            {
                CurrentUserId = request.CurrentUserId,
                Title = result.Title,
                Description = result.Description,
                SubjectId = result.SubjectId,
                DueDate = result.DueDate,
                IsGroupTask = result.IsGroupTask
            }, cancellationToken);

            return result;
        }

        var createdTaskId = await _sender.Send(new CreateTaskCommand
        {
            Title = normalizedTitle.Length > TaskBoardRules.TaskTitleMaxLength
                ? normalizedTitle[..TaskBoardRules.TaskTitleMaxLength]
                : normalizedTitle,
            Description = normalizedDescription,
            Deadline = request.DueDate,
            IsGroupTask = request.IsGroupTask,
            UserId = request.CurrentUserId.Value,
            Subject = new Subject
            {
                Id = selectedSubject!.Id,
                Name = selectedSubject.Name ?? string.Empty
            },
            ResourceUrl = request.ResourceUrl
        }, cancellationToken);

        result.IsSuccess = true;
        result.CreatedTaskId = createdTaskId;
        return result;
    }
}
