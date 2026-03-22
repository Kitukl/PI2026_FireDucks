using MediatR;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.Core.Tasks.Queries;

public record GetGroupedTaskStatsRequest : IRequest<Dictionary<string, Dictionary<string, int>>>;
    
public class GetGroupedTaskStatsHandler: IRequestHandler<GetGroupedTaskStatsRequest , Dictionary<string, Dictionary<string, int>>>
{
    private readonly ITaskRepository _repository;

    public GetGroupedTaskStatsHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Dictionary<string, Dictionary<string, int>>> Handle(GetGroupedTaskStatsRequest request, CancellationToken cancellationToken)
    {
        var stats = await _repository.GetGroupedTaskStatsAsync();
       
        return stats.ToDictionary(
            outer => outer.Key ? "Group Tasks" : "User Tasks",
            outer => outer.Value.ToDictionary(
                inner => inner.Key.ToString(),
                inner => inner.Value
            )
        );
    }
}