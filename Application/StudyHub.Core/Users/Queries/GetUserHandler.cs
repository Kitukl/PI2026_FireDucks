using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Users.Queries;

public record GetUserRequest(Guid Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUserRequest, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetUserById(request.Id);
        return new UserDto
        {
            GroupName = user.Group.Name,
            Name = user.Name,
            Surname = user.Surname,
            Roles = await _userRepository.GetRolesByUser(user)
        };
    }
}