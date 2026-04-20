using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Enums;
using System.Linq;

namespace StudyHub.Core.Admin.Queries;

public class GetAdminUpdateUserFormQuery : IRequest<AdminUpdateUserFormDataDto>
{
    public Guid UserId { get; set; }
    public string? GroupName { get; set; }
    public IEnumerable<string>? SelectedRoles { get; set; }
}

public class GetAdminUpdateUserFormQueryHandler : IRequestHandler<GetAdminUpdateUserFormQuery, AdminUpdateUserFormDataDto>
{
    private readonly ISender _sender;

    public GetAdminUpdateUserFormQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminUpdateUserFormDataDto> Handle(GetAdminUpdateUserFormQuery request, CancellationToken cancellationToken)
    {
        var userDto = await _sender.Send(new GetUserRequest(request.UserId), cancellationToken);
        var users = await _sender.Send(new GetUsersRequest(), cancellationToken);

        var existingGroups = users
            .Select(user => user.GroupName)
            .Where(groupName => !string.IsNullOrWhiteSpace(groupName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(groupName => groupName)
            .ToList();

        var availableRoles = Enum.GetNames(typeof(Role)).ToList();
        var selectedRoles = request.SelectedRoles?.Any() == true
            ? request.SelectedRoles
            : (userDto.Roles ?? new List<string>());

        return new AdminUpdateUserFormDataDto
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname ?? string.Empty,
            PhotoUrl = userDto.PhotoUrl ?? string.Empty,
            GroupName = request.GroupName ?? userDto.GroupName ?? string.Empty,
            Roles = userDto.Roles ?? new List<string>(),
            SelectedRoles = selectedRoles
                .Where(role => availableRoles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            AvailableRoles = availableRoles,
            ExistingGroups = existingGroups
        };
    }
}
