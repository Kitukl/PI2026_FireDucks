using MediatR;
using StudyHub.Core.Common;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Storage.Queries;

public class DownloadStorageFileQueryHandler : IRequestHandler<DownloadStorageFileQuery, StorageFileContentDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public DownloadStorageFileQueryHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<StorageFileContentDto> Handle(DownloadStorageFileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);

        var safeName = Path.GetFileName(request.Name);
        if (string.IsNullOrWhiteSpace(safeName))
        {
            return StorageFileContentDto.NotFound();
        }

        var containerName = StorageScopeHelper.ResolveContainerName(user, request.Scope);
        if (containerName == null)
        {
            return StorageFileContentDto.Forbidden();
        }

        var stream = await _blobService.GetFileAsync(containerName, safeName, cancellationToken);
        if (stream == null)
        {
            return StorageFileContentDto.NotFound();
        }

        await using (stream)
        using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream, cancellationToken);
            return new StorageFileContentDto
            {
                IsForbidden = false,
                IsNotFound = false,
                Content = memoryStream.ToArray(),
                ContentType = StorageNamingHelper.GetContentTypeByFileName(safeName),
                DownloadName = StorageNamingHelper.StripStoredFilePrefix(safeName)
            };
        }
    }
}