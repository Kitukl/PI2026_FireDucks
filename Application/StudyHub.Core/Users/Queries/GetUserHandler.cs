using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Queries;

public record GetUserRequest(Guid Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUserRequest, UserDto>
{
    private readonly IUserRepository userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        return await userRepository.GetUserById(request.Id);
    }
}