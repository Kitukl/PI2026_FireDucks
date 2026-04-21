using MediatR;

namespace StudyHub.Core.Storage.Commands;

public class RenameStorageFileCommand : IRequest<StorageOperationResult>
{
    public Guid UserId { get; set; }
    public string Scope { get; set; } = "personal";
    public string Name { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
}