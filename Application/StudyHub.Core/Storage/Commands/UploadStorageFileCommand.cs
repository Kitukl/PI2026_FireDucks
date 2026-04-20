using MediatR;

namespace StudyHub.Core.Storage.Commands;

public class UploadStorageFileCommand : IRequest<StorageOperationResult>
{
    public Guid UserId { get; set; }
    public bool ShareWithGroup { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public byte[] Content { get; set; } = [];
}