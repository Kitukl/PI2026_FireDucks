using Application.Models;
using StudyHub.Core.DTOs;
using StudyHub.Core.Storage.DTOs;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.Core.Common;

public static class StorageHelper
{
    private const string UserStoragePrefix = "user-storage-";
    private const string GroupStoragePrefix = "group-storage-";

    public static async Task<StoragePageViewModel> BuildStoragePageModelAsync(UserDto user, IBlobService blobService, CancellationToken cancellationToken)
    {
        var model = new StoragePageViewModel();

        var personalContainer = BuildUserStorageContainerName(user.Id);
        var personalFiles = await blobService.ListFilesAsync(personalContainer, cancellationToken);
        model.Files.AddRange(personalFiles.Select(file => MapStorageFile(file, "personal", false)));

        if (!string.IsNullOrWhiteSpace(user.GroupName))
        {
            model.GroupName = user.GroupName;
            model.CanShareWithGroup = true;

            var groupContainer = BuildGroupStorageContainerName(user.GroupName);
            var groupFiles = await blobService.ListFilesAsync(groupContainer, cancellationToken);
            model.Files.AddRange(groupFiles.Select(file => MapStorageFile(file, "group", true)));
        }

        model.Files = model.Files
            .OrderByDescending(item => item.LastModified)
            .ThenBy(item => item.Name)
            .ToList();

        return model;
    }

    private static StorageFileItemViewModel MapStorageFile(BlobFileInfoDto blobFile, string scope, bool isGroupFile)
    {
        var rawName = Path.GetFileName(blobFile.Name);
        var cleanName = StripStoredFilePrefix(rawName);
        var ext = Path.GetExtension(cleanName).TrimStart('.').ToUpperInvariant();

        return new StorageFileItemViewModel
        {
            Name = cleanName,
            BlobName = rawName,
            DownloadName = cleanName,
            Scope = scope,
            Extension = string.IsNullOrWhiteSpace(ext) ? "-" : ext,
            SizeDisplay = FormatFileSize(blobFile.Size),
            LastModified = blobFile.LastModified,
            LastModifiedDisplay = blobFile.LastModified?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "-",
            CanPreview = CanPreviewFileByExtension(ext),
            IsGroupFile = isGroupFile
        };
    }

    private static string BuildUserStorageContainerName(Guid userId)
    {
        return $"{UserStoragePrefix}{userId:D}";
    }

    private static string BuildGroupStorageContainerName(string groupName)
    {
        return $"{GroupStoragePrefix}{groupName}";
    }

    private static string StripStoredFilePrefix(string fileName)
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

    private static bool CanPreviewFileByExtension(string extension)
    {
        return extension switch
        {
            "PNG" => true,
            "JPG" => true,
            "JPEG" => true,
            "WEBP" => true,
            "GIF" => true,
            "BMP" => true,
            "PDF" => true,
            "TXT" => true,
            "CSV" => true,
            _ => false
        };
    }

    private static string FormatFileSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double size = bytes;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.#} {units[unitIndex]}";
    }
}