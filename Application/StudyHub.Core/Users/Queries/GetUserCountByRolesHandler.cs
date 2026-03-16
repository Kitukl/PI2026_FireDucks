using MediatR;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Queries;

public record GetUserCountByRole : IRequest<Dictionary<string, int>>;
    
public class GetUserCountByRolesHandler : IRequestHandler<GetUserCountByRole , Dictionary<string,int>>
{
    private readonly IUserRepository repository;
    
    public GetUserCountByRolesHandler(IUserRepository repository)
    {
         this.repository = repository;
    }
    
    public async Task<Dictionary<string, int>> Handle(GetUserCountByRole request, CancellationToken cancellationToken)
    {
        return await repository.GetUsersCountByRoleAsync();
    }
}