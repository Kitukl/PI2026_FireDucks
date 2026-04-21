namespace StudyHub.Core.Common;

public static class StorageNamingHelper
{
    private const string UserStoragePrefix = "user-storage-";
    private const string GroupStoragePrefix = "group-storage-";

    public static string BuildUserStorageContainerName(Guid userId) => $"{UserStoragePrefix}{userId:D}";

    public static string BuildGroupStorageContainerName(string groupName) => $"{GroupStoragePrefix}{groupName}";

    public static string BuildStoredBlobName(string originalName) => $"{Guid.NewGuid():N}__{originalName}";

    public static string StripStoredFilePrefix(string fileName)
    {
        var normalized = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return fileName;
        }

        var separatorIndex = normalized.IndexOf("__", StringComparison.Ordinal);
        if (separatorIndex > 0 && separatorIndex + 2 < normalized.Length)
        {
            return normalized[(separatorIndex + 2)..];
        }

        var dashIndex = normalized.IndexOf('-', StringComparison.Ordinal);
        if (dashIndex > 20 && dashIndex + 1 < normalized.Length)
        {
            return normalized[(dashIndex + 1)..];
        }

        return normalized;
    }

    public static string GetContentTypeByFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }
}