using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Queries;

public record GetUsersRequest : IRequest<IEnumerable<UserDto>>;

public class GetUsersHandler : IRequestHandler<GetUsersRequest, IEnumerable<UserDto>>
{
    private readonly IUserRepository userRepository;
    
    public GetUsersHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task<IEnumerable<UserDto>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        return await userRepository.GetUsersAsync();
    }
}