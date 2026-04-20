using MediatR;
using StudyHub.Core.Group;

namespace StudyHub.Core.Schedules.Commands;

public class RunGlobalScheduleUpdateCommandHandler : IRequestHandler<RunGlobalScheduleUpdateRequest>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ISender _sender;

    public RunGlobalScheduleUpdateCommandHandler(IGroupRepository groupRepository, ISender sender)
    {
        _groupRepository = groupRepository;
        _sender = sender;
    }

    public async Task Handle(RunGlobalScheduleUpdateRequest request, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetAllGroupsAsync();
        foreach (var group in groups.Where(g => !string.IsNullOrWhiteSpace(g.Name)))
        {
            await _sender.Send(new ParseAndSaveScheduleCommand(group.Name), cancellationToken);
        }
    }
}