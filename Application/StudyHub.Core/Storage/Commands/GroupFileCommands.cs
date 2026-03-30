using MediatR;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.Core.Storage.Commands;

public class UploadGroupFileCommand : IRequest<string>
{
    public Guid GroupId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string? ContentType { get; set; }
}

public class UploadGroupFileCommandHandler : IRequestHandler<UploadGroupFileCommand, string>
{
    private readonly IBlobService _blobService;

    public UploadGroupFileCommandHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<string> Handle(UploadGroupFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Content.Length == 0)
        {
            throw new InvalidOperationException("File content is empty.");
        }

        using var stream = new MemoryStream(request.Content);
        return await _blobService.UploadFileAsync(
            request.GroupId.ToString("D"),
            request.FileName,
            stream,
            request.ContentType,
            cancellationToken);
    }
}

public class DeleteGroupFileCommand : IRequest
{
    public Guid GroupId { get; set; }
    public string FileName { get; set; } = string.Empty;
}

public class DeleteGroupFileCommandHandler : IRequestHandler<DeleteGroupFileCommand>
{
    private readonly IBlobService _blobService;

    public DeleteGroupFileCommandHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task Handle(DeleteGroupFileCommand request, CancellationToken cancellationToken)
    {
        await _blobService.DeleteFileAsync(
            request.GroupId.ToString("D"),
            request.FileName,
            cancellationToken);
    }
}
