using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;

namespace StudyHub.Core.Admin.Queries;

public record GetAdminDashboardQuery : IRequest<AdminDashboardDataDto>;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDataDto>
{
    private readonly ISender _sender;

    public GetAdminDashboardQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminDashboardDataDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var userStats = await _sender.Send(new GetUsersStatisticRequest(), cancellationToken);
        var tasksCount = await _sender.Send(new GetTaskCountRequest(), cancellationToken);
        var taskStatusCount = await _sender.Send(new GetGroupedTaskStatsRequest(), cancellationToken);

        return new AdminDashboardDataDto
        {
            CreatedAt = userStats.CreatedAt,
            UserActivityPerMonth = userStats.UserActivityPerMonth,
            StudentsCount = userStats.StudentsCount,
            GroupsCount = userStats.GroupsCount,
            LeadersCount = userStats.LeadersCount,
            UserFilesCount = userStats.UserFilesCount,
            GroupFilesCount = userStats.GroupFilesCount,
            FileCount = userStats.FileCount,
            TaskCount = tasksCount,
            GroupedTaskCount = taskStatusCount
        };
    }
}
