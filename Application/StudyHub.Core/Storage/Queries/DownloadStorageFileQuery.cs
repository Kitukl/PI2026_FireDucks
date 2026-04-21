using MediatR;

namespace StudyHub.Core.Storage.Queries;

public class DownloadStorageFileQuery : IRequest<StorageFileContentDto>
{
    public Guid UserId { get; set; }
    public string Scope { get; set; } = "personal";
    public string Name { get; set; } = string.Empty;
}