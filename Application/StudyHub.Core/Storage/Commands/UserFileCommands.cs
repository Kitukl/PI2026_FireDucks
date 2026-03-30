using MediatR;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.Core.Storage.Commands;

public class UploadUserFileCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string? ContentType { get; set; }
}

public class UploadUserFileCommandHandler : IRequestHandler<UploadUserFileCommand, string>
{
    private readonly IBlobService _blobService;

    public UploadUserFileCommandHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<string> Handle(UploadUserFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Content.Length == 0)
        {
            throw new InvalidOperationException("File content is empty.");
        }

        using var stream = new MemoryStream(request.Content);
        return await _blobService.UploadFileAsync(
            request.UserId.ToString("D"),
            request.FileName,
            stream,
            request.ContentType,
            cancellationToken);
    }
}

public class DeleteUserFileCommand : IRequest
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
}

public class DeleteUserFileCommandHandler : IRequestHandler<DeleteUserFileCommand>
{
    private readonly IBlobService _blobService;

    public DeleteUserFileCommandHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task Handle(DeleteUserFileCommand request, CancellationToken cancellationToken)
    {
        await _blobService.DeleteFileAsync(
            request.UserId.ToString("D"),
            request.FileName,
            cancellationToken);
    }
}
