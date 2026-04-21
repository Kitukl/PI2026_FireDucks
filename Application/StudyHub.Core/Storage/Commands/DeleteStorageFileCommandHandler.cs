using MediatR;
using StudyHub.Core.Common;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Storage.Commands;

public class DeleteStorageFileCommandHandler : IRequestHandler<DeleteStorageFileCommand, StorageOperationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public DeleteStorageFileCommandHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<StorageOperationResult> Handle(DeleteStorageFileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);

        var safeName = Path.GetFileName(request.Name);
        if (string.IsNullOrWhiteSpace(safeName))
        {
            return StorageOperationResult.Fail("File name is required.");
        }

        var containerName = StorageScopeHelper.ResolveContainerName(user, request.Scope);
        if (containerName == null)
        {
            return StorageOperationResult.Forbidden();
        }

        await _blobService.DeleteFileAsync(containerName, safeName, cancellationToken);
        return StorageOperationResult.Success("File deleted.");
    }
}