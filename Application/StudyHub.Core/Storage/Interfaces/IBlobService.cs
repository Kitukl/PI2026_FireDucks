namespace StudyHub.Core.Storage.Interfaces;

using StudyHub.Core.Storage.DTOs;

public interface IBlobService
{
    Task<string> UploadFileAsync(
        string containerName,
        string fileName,
        Stream content,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    Task<Stream?> GetFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlobFileInfoDto>> ListFilesAsync(
        string containerName,
        CancellationToken cancellationToken = default);
}
