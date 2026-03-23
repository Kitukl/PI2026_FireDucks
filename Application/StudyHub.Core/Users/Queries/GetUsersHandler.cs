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
            Id =  c.Id,
            Surname =  c.Surname,
            Name = c.Name,
            GroupName = c.Group?.Name,
            Roles = _userRepository.GetRolesByUser(c).Result,
        }).ToList();
    }
}