using MediatR;
using StudyHub.Core.Common;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Storage.Commands;

public class UploadStorageFileCommandHandler : IRequestHandler<UploadStorageFileCommand, StorageOperationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public UploadStorageFileCommandHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<StorageOperationResult> Handle(UploadStorageFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Content.Length == 0)
        {
            return StorageOperationResult.Fail("Please choose a file to upload.");
        }

        var user = await _userRepository.GetUserById(request.UserId);

        var originalName = Path.GetFileName(request.FileName);
        if (string.IsNullOrWhiteSpace(originalName))
        {
            originalName = $"file-{Guid.NewGuid():N}";
        }

        var targetContainer = StorageNamingHelper.BuildUserStorageContainerName(user.Id);
        var isGroupUpload = false;

        if (request.ShareWithGroup && !string.IsNullOrWhiteSpace(user.Group?.Name))
        {
            targetContainer = StorageNamingHelper.BuildGroupStorageContainerName(user.Group.Name);
            isGroupUpload = true;
        }

        var blobName = StorageNamingHelper.BuildStoredBlobName(originalName);

        await using var stream = new MemoryStream(request.Content);
        await _blobService.UploadFileAsync(targetContainer, blobName, stream, request.ContentType, cancellationToken);

        return StorageOperationResult.Success(isGroupUpload
            ? "File uploaded to your group storage."
            : "File uploaded to your personal storage.");
    }
}