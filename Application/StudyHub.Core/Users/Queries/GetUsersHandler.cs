using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Queries;

public record GetUsersRequest : IRequest<IEnumerable<UserDto>>;

public class GetUsersHandler : IRequestHandler<GetUsersRequest, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    
    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<UserDto>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response =  await _userRepository.GetUsersAsync();
        return response.Select(c => new UserDto
        {
            GroupName = c.Group.Name,
            Name = c.Name,
            Surname = c.Surname
        });
    }
}