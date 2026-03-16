using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Statistics.Interfaces;

namespace StudyHub.Core.Statistics.Queries;

public record GetUsersStatisticRequest() : IRequest<UsersStatisticDto>;

public class GetUsersStatisticHandler : IRequestHandler<GetUsersStatisticRequest,UsersStatisticDto>
{
    private readonly IStatisticRepository repository;
    public GetUsersStatisticHandler(IStatisticRepository repository)
    { 
        this.repository = repository;
    }
    
    public async Task<UsersStatisticDto> Handle(GetUsersStatisticRequest request, CancellationToken cancellationToken)
    {
        var rawData = await repository.GetRecentStatisticAsync();
        
        if (rawData is null)
        {
            return new UsersStatisticDto(DateTime.Now, [], 0); 
        }
        
        var userActivityPerMonth = await repository.GetYearlyActivityAsync(DateTime.Now.Year);

        return new UsersStatisticDto(
            rawData.CreatedAt,
            userActivityPerMonth,
            rawData.FilesCount
        );
    }
}