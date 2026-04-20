using MediatR;

namespace StudyHub.Core.Schedules.Commands;

public record UpdateGroupScheduleRequest(Guid GroupId) : IRequest<bool>;