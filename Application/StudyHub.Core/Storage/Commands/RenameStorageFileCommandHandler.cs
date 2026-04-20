using MediatR;
using StudyHub.Core.Common;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Storage.Commands;

public class RenameStorageFileCommandHandler : IRequestHandler<RenameStorageFileCommand, StorageOperationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public RenameStorageFileCommandHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<StorageOperationResult> Handle(RenameStorageFileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);

        var safeOldName = Path.GetFileName(request.Name);
        var safeNewName = Path.GetFileName(request.NewName)?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(safeOldName) || string.IsNullOrWhiteSpace(safeNewName))
        {
            return StorageOperationResult.Fail("File name is required.");
        }

        var oldExtension = Path.GetExtension(safeOldName) ?? string.Empty;
        var newExtension = Path.GetExtension(safeNewName) ?? string.Empty;
        if (!string.Equals(oldExtension, newExtension, StringComparison.OrdinalIgnoreCase))
        {
            return StorageOperationResult.Fail("Changing file extension is not allowed.");
        }

        var containerName = StorageScopeHelper.ResolveContainerName(user, request.Scope);
        if (containerName == null)
        {
            return StorageOperationResult.Forbidden();
        }

        var sourceStream = await _blobService.GetFileAsync(containerName, safeOldName, cancellationToken);
        if (sourceStream == null)
        {
            return StorageOperationResult.Fail("File not found.");
        }

        await using (sourceStream)
        {
            var targetBlobName = StorageNamingHelper.BuildStoredBlobName(safeNewName);
            await _blobService.UploadFileAsync(
                containerName,
                targetBlobName,
                sourceStream,
                StorageNamingHelper.GetContentTypeByFileName(safeNewName),
                cancellationToken);
        }

        await _blobService.DeleteFileAsync(containerName, safeOldName, cancellationToken);

        return StorageOperationResult.Success("File renamed.");
    }
}