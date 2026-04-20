using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Enums;
using System.Linq;

namespace StudyHub.Core.Admin.Commands;

public class UpdateAdminUserCommand : IRequest<AdminUserUpdateResultDto>
{
    public Guid UserId { get; set; }
    public string? GroupName { get; set; }
    public IEnumerable<string>? SelectedRoles { get; set; }
}

public class UpdateAdminUserCommandHandler : IRequestHandler<UpdateAdminUserCommand, AdminUserUpdateResultDto>
{
    private readonly ISender _sender;

    public UpdateAdminUserCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminUserUpdateResultDto> Handle(UpdateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedSelectedRoles = (request.SelectedRoles ?? Enumerable.Empty<string>())
            .Where(role => Enum.GetNames(typeof(Role)).Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var result = new AdminUserUpdateResultDto
        {
            UserId = request.UserId,
            GroupName = request.GroupName?.Trim() ?? string.Empty,
            Roles = normalizedSelectedRoles
        };

        if (normalizedSelectedRoles.Count == 0)
        {
            result.Errors.Add("Changes cannot be saved. Please add at least one role.");
        }

        var hasAdminRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase));
        var hasStudentRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Student), StringComparison.OrdinalIgnoreCase));
        var hasLeaderRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Leader), StringComparison.OrdinalIgnoreCase));

        if (hasLeaderRole && !hasStudentRole)
        {
            result.Errors.Add("Changes cannot be saved. Leader should has Student role.");
        }

        if (hasAdminRole && (hasStudentRole || hasLeaderRole))
        {
            result.Errors.Add("Changes cannot be saved. Admin role cannot be provided to Student.");
        }

        if (result.Errors.Count > 0)
        {
            return result;
        }

        var userDto = await _sender.Send(new GetUserRequest(request.UserId), cancellationToken);
        var existingRoles = (userDto.Roles ?? new List<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var rolesToRemove = existingRoles
            .Where(role => !normalizedSelectedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var rolesToAdd = normalizedSelectedRoles
            .Where(role => !existingRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var role in rolesToRemove)
        {
            if (!Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                continue;
            }

            await _sender.Send(new RemoveUserRoleCommand
            {
                UserId = request.UserId,
                Role = parsedRole
            }, cancellationToken);
        }

        foreach (var role in rolesToAdd)
        {
            if (!Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                continue;
            }

            await _sender.Send(new AssignUserRoleCommand
            {
                UserId = request.UserId,
                Role = parsedRole
            }, cancellationToken);
        }

        await _sender.Send(new UpdateUserCommand
        {
            Id = request.UserId,
            GroupName = request.GroupName?.Trim() ?? string.Empty
        }, cancellationToken);

        result.IsSuccess = true;
        return result;
    }
}
