using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Queries;

public class GetTasksQuery : IRequest<IEnumerable<Task>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<Task>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Task>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTasksAsync();
    }
}