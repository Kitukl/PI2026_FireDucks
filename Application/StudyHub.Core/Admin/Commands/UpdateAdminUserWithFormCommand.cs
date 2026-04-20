using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.Admin.Queries;

namespace StudyHub.Core.Admin.Commands;

public class UpdateAdminUserWithFormCommand : IRequest<UpdateAdminUserWithFormResult>
{
    public Guid UserId { get; set; }
    public string? GroupName { get; set; }
    public IEnumerable<string>? SelectedRoles { get; set; }
}

public class UpdateAdminUserWithFormResult
{
    public bool IsSuccess { get; set; }
    public Guid UserId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool HasValidationErrors { get; set; }
    public List<string> Errors { get; set; } = new();
    public AdminUpdateUserFormDataDto? InvalidData { get; set; }
}

public class UpdateAdminUserWithFormCommandHandler : IRequestHandler<UpdateAdminUserWithFormCommand, UpdateAdminUserWithFormResult>
{
    private readonly ISender _sender;

    public UpdateAdminUserWithFormCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<UpdateAdminUserWithFormResult> Handle(UpdateAdminUserWithFormCommand request, CancellationToken cancellationToken)
    {
        var updateResult = await _sender.Send(new UpdateAdminUserCommand
        {
            UserId = request.UserId,
            GroupName = request.GroupName,
            SelectedRoles = request.SelectedRoles
        }, cancellationToken);

        var result = new UpdateAdminUserWithFormResult
        {
            IsSuccess = updateResult.IsSuccess,
            UserId = updateResult.UserId,
            GroupName = updateResult.GroupName,
            Roles = updateResult.Roles,
            Errors = updateResult.Errors,
            HasValidationErrors = !updateResult.IsSuccess
        };

        if (!updateResult.IsSuccess)
        {
            var invalidData = await _sender.Send(new GetAdminUpdateUserFormQuery
            {
                UserId = request.UserId,
                GroupName = request.GroupName,
                SelectedRoles = updateResult.Roles
            }, cancellationToken);

            result.InvalidData = invalidData;
        }

        return result;
    }
}
