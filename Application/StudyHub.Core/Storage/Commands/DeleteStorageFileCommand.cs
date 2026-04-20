using MediatR;

namespace StudyHub.Core.Storage.Commands;

public class DeleteStorageFileCommand : IRequest<StorageOperationResult>
{
    public Guid UserId { get; set; }
    public string Scope { get; set; } = "personal";
    public string Name { get; set; } = string.Empty;
}