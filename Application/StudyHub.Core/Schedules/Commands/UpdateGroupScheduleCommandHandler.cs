using MediatR;
using StudyHub.Core.Group;

namespace StudyHub.Core.Schedules.Commands;

public class UpdateGroupScheduleCommandHandler : IRequestHandler<UpdateGroupScheduleRequest, bool>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ISender _sender;

    public UpdateGroupScheduleCommandHandler(IGroupRepository groupRepository, ISender sender)
    {
        _groupRepository = groupRepository;
        _sender = sender;
    }

    public async Task<bool> Handle(UpdateGroupScheduleRequest request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupByIdAsync(request.GroupId);
        if (group == null || string.IsNullOrWhiteSpace(group.Name))
        {
            return false;
        }

        await _sender.Send(new ParseAndSaveScheduleCommand(group.Name), cancellationToken);
        return true;
    }
}