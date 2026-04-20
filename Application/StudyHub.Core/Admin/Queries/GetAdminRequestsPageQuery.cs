using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Admin.Queries;

public class GetAdminRequestsPageQuery : IRequest<AdminRequestsPageDataDto>
{
    public string? FeedbackId { get; set; }
    public bool OpenModal { get; set; }
}

public class GetAdminRequestsPageQueryHandler : IRequestHandler<GetAdminRequestsPageQuery, AdminRequestsPageDataDto>
{
    private readonly ISender _sender;

    public GetAdminRequestsPageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminRequestsPageDataDto> Handle(GetAdminRequestsPageQuery request, CancellationToken cancellationToken)
    {
        var feedbacks = await _sender.Send(new GetFeedbacksCommand(), cancellationToken);
        var allowedStatuses = new[] { Status.ToDo, Status.InProgress, Status.Resolved };

        var filteredRequests = feedbacks
            .Where(item => allowedStatuses.Contains(item.Status))
            .OrderByDescending(item => item.CreatedAt)
            .ToList();

        Feedback? activeRequest = null;
        if (!string.IsNullOrWhiteSpace(request.FeedbackId) && Guid.TryParse(request.FeedbackId, out var parsedId))
        {
            activeRequest = filteredRequests.FirstOrDefault(item => item.Id == parsedId);
            if (activeRequest == null)
            {
                try
                {
                    activeRequest = await _sender.Send(new GetFeedbackCommand { Id = parsedId }, cancellationToken);
                }
                catch
                {
                    activeRequest = null;
                }
            }
        }

        activeRequest ??= filteredRequests.FirstOrDefault();

        return new AdminRequestsPageDataDto
        {
            Requests = filteredRequests,
            ActiveRequest = activeRequest,
            OpenRequestModal = request.OpenModal && activeRequest != null
        };
    }
}
