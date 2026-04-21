using MediatR;

namespace StudyHub.Core.Group.Commands;

public class UpdateGroupCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, bool>
{
    private readonly IGroupRepository _groupRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<bool> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupRepository.UpdateGroupAsync(request.Id, request.Name);
    }
}