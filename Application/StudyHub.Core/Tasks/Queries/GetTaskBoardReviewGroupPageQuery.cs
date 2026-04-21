using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Tasks.Queries;

public class GetTaskBoardReviewGroupPageQuery : IRequest<TaskBoardReviewGroupPageDataDto>
{
    public Guid? CurrentUserId { get; set; }
}

public class GetTaskBoardReviewGroupPageQueryHandler : IRequestHandler<GetTaskBoardReviewGroupPageQuery, TaskBoardReviewGroupPageDataDto>
{
    private readonly ISender _sender;

    public GetTaskBoardReviewGroupPageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardReviewGroupPageDataDto> Handle(GetTaskBoardReviewGroupPageQuery request, CancellationToken cancellationToken)
    {
        var boardModel = await _sender.Send(new GetTaskBoardPageQuery
        {
            CurrentUserId = request.CurrentUserId
        }, cancellationToken);

        var users = (await _sender.Send(new GetUsersRequest(), cancellationToken)).ToList();

        var currentGroupName = "No group";
        if (request.CurrentUserId.HasValue)
        {
            var currentUser = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
            currentGroupName = string.IsNullOrWhiteSpace(currentUser.GroupName) ? "No group" : currentUser.GroupName;
        }

        var groupUsers = users
            .Where(user => string.Equals(user.GroupName, currentGroupName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new GroupUserDto
            {
                UserId = user.Id,
                Name = $"{user.Name} {user.Surname}".Trim(),
                Role = (user.Roles?.Any(role => string.Equals(role, nameof(Role.Leader), StringComparison.OrdinalIgnoreCase)) == true)
                    ? "Leader"
                    : "Student"
            })
            .ToList();

        var unassignedUsers = users
            .Where(user => string.IsNullOrWhiteSpace(user.GroupName))
            .Where(user => user.Roles?.Any(role => string.Equals(role, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase)) != true)
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new GroupUserDto
            {
                UserId = user.Id,
                Name = $"{user.Name} {user.Surname}".Trim(),
                Role = "Student"
            })
            .ToList();

        return new TaskBoardReviewGroupPageDataDto
        {
            Board = boardModel,
            GroupName = currentGroupName,
            GroupUsers = groupUsers,
            UnassignedUsers = unassignedUsers
        };
    }
}
