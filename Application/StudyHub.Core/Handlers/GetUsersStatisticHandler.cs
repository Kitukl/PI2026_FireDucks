using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Interfaces;
using StudyHub.Core.Queries;

namespace StudyHub.Core.Handlers;

public class GetUsersStatisticHandler(IStatisticRepository repo) : IRequestHandler<GetUsersStatisticQuery,UsersStatisticDto>
{
    public async Task<UsersStatisticDto> Handle(GetUsersStatisticQuery request, CancellationToken cancellationToken)
    {
        var rawData = await repo.GetRecentStatisticAsync();
        
        if (rawData is null)
        {
            return new UsersStatisticDto(DateTime.Now, 0, 0); 
        }
        
        return new UsersStatisticDto(
            rawData.CreatedAt,
            rawData.UserActivityPerMonth,
            rawData.FilesCount
        );
    }
}