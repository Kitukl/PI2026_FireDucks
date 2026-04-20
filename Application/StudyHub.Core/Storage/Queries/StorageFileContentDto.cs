namespace StudyHub.Core.Storage.Queries;

public class StorageFileContentDto
{
    public bool IsForbidden { get; set; }
    public bool IsNotFound { get; set; }
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = "application/octet-stream";
    public string DownloadName { get; set; } = string.Empty;

    public static StorageFileContentDto Forbidden() => new() { IsForbidden = true };
    public static StorageFileContentDto NotFound() => new() { IsNotFound = true };
}