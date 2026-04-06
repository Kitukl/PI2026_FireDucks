using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Storage.DTOs;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using Application.Models;

namespace Application.Controllers;

public class StorageController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;
    private readonly IBlobService _blobService;

    private const string UserStoragePrefix = "user-storage-";
    private const string GroupStoragePrefix = "group-storage-";

    public StorageController(
        UserManager<User> userManager,
        IMediator mediator,
        IBlobService blobService)
    {
        _userManager = userManager;
        _mediator = mediator;
        _blobService = blobService;
    }

    [HttpGet("/Storage")]
    public async Task<IActionResult> Storage(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        var model = await BuildStoragePageModelAsync(user, cancellationToken);
        return View("~/Views/Home/Storage/Storage.cshtml", model);
    }

    [HttpPost("/Storage/Upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadStorageFile(IFormFile? uploadFile, bool shareWithGroup, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (uploadFile is not { Length: > 0 })
        {
            TempData["StorageError"] = "Please choose a file to upload.";
            return RedirectToAction(nameof(Storage));
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        var hasGroup = !string.IsNullOrWhiteSpace(userDto.GroupName);

        var targetContainer = BuildUserStorageContainerName(user.Id);
        var isGroupUpload = false;
        if (shareWithGroup && hasGroup)
        {
            targetContainer = BuildGroupStorageContainerName(userDto.GroupName!);
            isGroupUpload = true;
        }

        var originalName = Path.GetFileName(uploadFile.FileName);
        if (string.IsNullOrWhiteSpace(originalName))
        {
            originalName = $"file-{Guid.NewGuid():N}";
        }

        var blobName = BuildStoredBlobName(originalName);
        await using var stream = uploadFile.OpenReadStream();
        await _blobService.UploadFileAsync(targetContainer, blobName, stream, uploadFile.ContentType, cancellationToken);

        TempData["StorageMessage"] = isGroupUpload
            ? "File uploaded to your group storage."
            : "File uploaded to your personal storage.";

        return RedirectToAction(nameof(Storage));
    }

    [HttpGet("/Storage/Download")]
    public async Task<IActionResult> DownloadStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return NotFound();
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        var stream = await _blobService.GetFileAsync(containerName, safeName, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        return File(stream, GetContentTypeByFileName(safeName), StripStoredFilePrefix(safeName));
    }

    [HttpGet("/Storage/Preview")]
    public async Task<IActionResult> PreviewStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return NotFound();
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        var stream = await _blobService.GetFileAsync(containerName, safeName, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        return File(stream, GetContentTypeByFileName(safeName));
    }

    [HttpPost("/Storage/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["StorageError"] = "File name is required.";
            return RedirectToAction(nameof(Storage));
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        await _blobService.DeleteFileAsync(containerName, safeName, cancellationToken);

        TempData["StorageMessage"] = "File deleted.";
        return RedirectToAction(nameof(Storage));
    }

    private async Task<StoragePageViewModel> BuildStoragePageModelAsync(User user, CancellationToken cancellationToken)
    {
        var model = new StoragePageViewModel();
        var currentUser = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);

        var personalContainer = BuildUserStorageContainerName(user.Id);
        var personalFiles = await _blobService.ListFilesAsync(personalContainer, cancellationToken);
        model.Files.AddRange(personalFiles.Select(file => MapStorageFile(file, "personal", false)));

        if (!string.IsNullOrWhiteSpace(currentUser.GroupName))
        {
            model.GroupName = currentUser.GroupName;
            model.CanShareWithGroup = true;

            var groupContainer = BuildGroupStorageContainerName(currentUser.GroupName);
            var groupFiles = await _blobService.ListFilesAsync(groupContainer, cancellationToken);
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

    private static string BuildStoredBlobName(string originalName)
    {
        return $"{Guid.NewGuid():N}__{originalName}";
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

    private static string GetContentTypeByFileName(string fileName)
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
