using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using StudyHub.Core.Storage.DTOs;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.Infrastructure.Storage;

public class BlobService : IBlobService
{
    private const string DefaultAzuriteConnectionString = "UseDevelopmentStorage=true";

    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BlobStorage");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = DefaultAzuriteConnectionString;
        }
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(
        string containerName,
        string fileName,
        Stream content,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        var safeContainerName = NormalizeContainerName(containerName);
        var safeFileName = NormalizeBlobName(fileName);

        var containerClient = await EnsureContainerExistsAsync(safeContainerName, cancellationToken);
        var blobClient = containerClient.GetBlobClient(safeFileName);

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        var uploadOptions = new BlobUploadOptions();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            uploadOptions.HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };
        }

        await blobClient.UploadAsync(content, uploadOptions, cancellationToken);

        return $"{safeContainerName}/{safeFileName}";
    }

    public async Task<Stream?> GetFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var safeContainerName = NormalizeContainerName(containerName);
        var safeFileName = NormalizeBlobName(fileName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(safeContainerName);
        var blobClient = containerClient.GetBlobClient(safeFileName);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return null;
        }

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        var output = new MemoryStream();
        await response.Value.Content.CopyToAsync(output, cancellationToken);
        output.Position = 0;

        return output;
    }

    public async Task DeleteFileAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var safeContainerName = NormalizeContainerName(containerName);
        var safeFileName = NormalizeBlobName(fileName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(safeContainerName);
        var blobClient = containerClient.GetBlobClient(safeFileName);

        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<BlobFileInfoDto>> ListFilesAsync(
        string containerName,
        CancellationToken cancellationToken = default)
    {
        var safeContainerName = NormalizeContainerName(containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(safeContainerName);

        if (!await containerClient.ExistsAsync(cancellationToken))
        {
            return Array.Empty<BlobFileInfoDto>();
        }

        var files = new List<BlobFileInfoDto>();

        await foreach (var blobItem in containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            files.Add(new BlobFileInfoDto
            {
                Name = blobItem.Name,
                Size = blobItem.Properties.ContentLength ?? 0,
                LastModified = blobItem.Properties.LastModified,
                ContentType = blobItem.Properties.ContentType
            });
        }

        return files;
    }

    private async Task<BlobContainerClient> EnsureContainerExistsAsync(string containerName, CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        return containerClient;
    }

    private static string NormalizeContainerName(string containerName)
    {
        var normalized = (containerName ?? string.Empty).Trim().ToLowerInvariant();
        normalized = Regex.Replace(normalized, "[^a-z0-9-]", "-");
        normalized = Regex.Replace(normalized, "-{2,}", "-").Trim('-');

        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = "default-container";
        }

        if (normalized.Length < 3)
        {
            normalized = normalized.PadRight(3, 'x');
        }

        if (normalized.Length > 63)
        {
            normalized = normalized[..63].Trim('-');
            if (normalized.Length < 3)
            {
                normalized = "container-xxx";
            }
        }

        return normalized;
    }

    private static string NormalizeBlobName(string fileName)
    {
        var safeName = Path.GetFileName(fileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = $"file-{Guid.NewGuid():N}";
        }

        return safeName;
    }
}