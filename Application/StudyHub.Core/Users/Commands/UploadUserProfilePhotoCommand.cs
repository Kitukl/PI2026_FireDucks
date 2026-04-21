using Application.Helpers;
using MediatR;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class UploadUserProfilePhotoCommand : IRequest<UploadUserProfilePhotoResult>
{
    public Guid? UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public byte[] Content { get; set; } = [];
}

public class UploadUserProfilePhotoResult
{
    public bool IsForbidden { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

public class UploadUserProfilePhotoCommandHandler : IRequestHandler<UploadUserProfilePhotoCommand, UploadUserProfilePhotoResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobService _blobService;

    public UploadUserProfilePhotoCommandHandler(IUserRepository userRepository, IBlobService blobService)
    {
        _userRepository = userRepository;
        _blobService = blobService;
    }

    public async Task<UploadUserProfilePhotoResult> Handle(UploadUserProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return new UploadUserProfilePhotoResult { IsForbidden = true };
        }

        if (request.Content.Length == 0)
        {
            return new UploadUserProfilePhotoResult { ErrorMessage = "Please select an image first." };
        }

        var user = await _userRepository.GetUserById(request.UserId.Value);

        var safeFileName = Path.GetFileName(request.FileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            safeFileName = $"avatar-{Guid.NewGuid():N}.bin";
        }

        var blobName = $"{user.Id:D}/{Guid.NewGuid():N}-{safeFileName}";

        await using var stream = new MemoryStream(request.Content);
        await _blobService.UploadFileAsync(
            UserProfileHelper.UserAvatarContainer,
            blobName,
            stream,
            request.ContentType,
            cancellationToken);

        if (UserProfileHelper.TryExtractAvatarBlobName(user.PhotoUrl, out var oldBlobName))
        {
            await _blobService.DeleteFileAsync(UserProfileHelper.UserAvatarContainer, oldBlobName, cancellationToken);
        }

        user.PhotoUrl = $"{UserProfileHelper.UserAvatarContainer}/{blobName}";
        await _userRepository.Update(user);

        return new UploadUserProfilePhotoResult
        {
            IsSuccess = true,
            SuccessMessage = "Profile photo updated."
        };
    }
}
