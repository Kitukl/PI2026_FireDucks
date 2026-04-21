using MediatR;
using Application.Models;
using StudyHub.Core.Common;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Core.DTOs;

namespace StudyHub.Core.Storage.Queries;

public class GetStoragePageDataQueryHandler : IRequestHandler<GetStoragePageDataQuery, StoragePageViewModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public GetStoragePageDataQueryHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<StoragePageViewModel> Handle(GetStoragePageDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);

        var userDto = new UserDto
        {
            Id = user.Id,
            GroupName = user.Group?.Name
        };

        return await StorageHelper.BuildStoragePageModelAsync(userDto, _blobService, cancellationToken);
    }
}