using Application.Helpers;
using MediatR;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.Core.Users.Queries;

public class DownloadUserProfilePhotoQuery : IRequest<DownloadUserProfilePhotoResult>
{
    public string? Path { get; set; }
}

public class DownloadUserProfilePhotoResult
{
    public bool IsNotFound { get; set; }
    public Stream? Content { get; set; }
    public string ContentType { get; set; } = "application/octet-stream";
}

public class DownloadUserProfilePhotoQueryHandler : IRequestHandler<DownloadUserProfilePhotoQuery, DownloadUserProfilePhotoResult>
{
    private readonly IBlobService _blobService;

    public DownloadUserProfilePhotoQueryHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<DownloadUserProfilePhotoResult> Handle(DownloadUserProfilePhotoQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Path))
        {
            return new DownloadUserProfilePhotoResult { IsNotFound = true };
        }

        var marker = UserProfileHelper.UserAvatarContainer + "/";
        var blobName = request.Path.StartsWith(marker, StringComparison.OrdinalIgnoreCase)
            ? request.Path[marker.Length..]
            : request.Path;

        var fileStream = await _blobService.GetFileAsync(UserProfileHelper.UserAvatarContainer, blobName, cancellationToken);
        if (fileStream == null)
        {
            return new DownloadUserProfilePhotoResult { IsNotFound = true };
        }

        return new DownloadUserProfilePhotoResult
        {
            Content = fileStream,
            ContentType = UserProfileHelper.GetImageContentType(blobName)
        };
    }
}
