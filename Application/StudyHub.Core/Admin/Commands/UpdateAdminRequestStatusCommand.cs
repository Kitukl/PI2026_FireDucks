using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Admin.Commands;

public class UpdateAdminRequestStatusCommand : IRequest<AdminFeedbackStatusUpdateResultDto>
{
    public Guid FeedbackId { get; set; }
    public Status Status { get; set; }
}

public class UpdateAdminRequestStatusCommandHandler : IRequestHandler<UpdateAdminRequestStatusCommand, AdminFeedbackStatusUpdateResultDto>
{
    private readonly ISender _sender;

    public UpdateAdminRequestStatusCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminFeedbackStatusUpdateResultDto> Handle(UpdateAdminRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var allowedStatuses = new[] { Status.ToDo, Status.InProgress, Status.Resolved };
        if (!allowedStatuses.Contains(request.Status))
        {
            return new AdminFeedbackStatusUpdateResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Unsupported status."
            };
        }

        await _sender.Send(new UpdateFeedbackCommand
        {
            Id = request.FeedbackId,
            Status = request.Status
        }, cancellationToken);

        return new AdminFeedbackStatusUpdateResultDto
        {
            IsSuccess = true,
            Status = request.Status.ToString(),
            StatusLabel = request.Status switch
            {
                Status.ToDo => "To do",
                Status.InProgress => "In progress",
                Status.Resolved => "Resolved",
                _ => request.Status.ToString()
            }
        };
    }
}
