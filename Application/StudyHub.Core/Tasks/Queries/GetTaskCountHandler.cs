using MediatR;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.Core.Tasks.Queries;

public record GetTaskCountRequest() : IRequest<int>;

public class GetTaskCountHandler : IRequestHandler<GetTaskCountRequest, int>
{
    private readonly ITaskRepository repository; 
        
    public GetTaskCountHandler(ITaskRepository repository)
    {
        this.repository = repository;

    }
    public async Task<int> Handle(GetTaskCountRequest request, CancellationToken cancellationToken)
    {
        return await repository.GetCountAsync();
    }
}