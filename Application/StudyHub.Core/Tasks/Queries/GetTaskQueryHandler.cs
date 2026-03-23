using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Queries;

public class GetTaskQuery : IRequest<Task>
{
    public Guid Id { get; set; }
}

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, Task>
{
    private readonly ITaskRepository _repository;

    public GetTaskQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    public async Task<Task> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTaskAsync(request.Id) ?? throw new Exception("Task not found");
    }
}