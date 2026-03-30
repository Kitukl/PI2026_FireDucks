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
            var emptyStorageFileCount = await repository.GetStorageFileCountAsync(cancellationToken);
            return new UsersStatisticDto(DateTime.Now, [], emptyStorageFileCount);
        }
        
        var userActivityPerMonth = await repository.GetYearlyActivityAsync(DateTime.Now.Year);
        var storageFileCount = await repository.GetStorageFileCountAsync(cancellationToken);

        return new UsersStatisticDto(
            rawData.CreatedAt,
            userActivityPerMonth,
            storageFileCount
        );
    }
}