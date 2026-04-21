namespace Application.Models;

public class StoragePageViewModel
{
    public string GroupName { get; set; } = string.Empty;
    public bool CanShareWithGroup { get; set; }
    public List<StorageFileItemViewModel> Files { get; set; } = new();
}

public class StorageFileItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string DownloadName { get; set; } = string.Empty;
    public string Scope { get; set; } = "personal";
    public string Extension { get; set; } = "-";
    public string SizeDisplay { get; set; } = "0 B";
    public DateTimeOffset? LastModified { get; set; }
    public string LastModifiedDisplay { get; set; } = "-";
    public bool CanPreview { get; set; }
    public bool IsGroupFile { get; set; }
}
