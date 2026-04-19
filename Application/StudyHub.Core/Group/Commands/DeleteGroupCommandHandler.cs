using MediatR;

namespace StudyHub.Core.Group.Commands;

public class DeleteGroupCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupRepository.DeleteGroupAsync(request.Id);
    }
}