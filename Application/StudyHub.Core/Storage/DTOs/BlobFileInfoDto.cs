namespace StudyHub.Core.Storage.DTOs;

public class BlobFileInfoDto
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? ContentType { get; set; }
}
