using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Storage.Commands;
using StudyHub.Core.Storage.Queries;

namespace Application.Controllers;

public class StorageController : Controller
{
    private readonly IMediator _mediator;

    public StorageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/Storage")]
    public async Task<IActionResult> Storage(CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = await _mediator.Send(new GetStoragePageDataQuery
        {
            UserId = currentUserId
        }, cancellationToken);

        return View("~/Views/Home/Storage/Storage.cshtml", model);
    }

    [HttpPost("/Storage/Upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadStorageFile(IFormFile? uploadFile, bool shareWithGroup, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (uploadFile is not { Length: > 0 })
        {
            TempData["StorageError"] = "Please choose a file to upload.";
            return RedirectToAction(nameof(Storage));
        }

        await using var stream = uploadFile.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        var result = await _mediator.Send(new UploadStorageFileCommand
        {
            UserId = currentUserId,
            ShareWithGroup = shareWithGroup,
            FileName = uploadFile.FileName,
            ContentType = uploadFile.ContentType,
            Content = memoryStream.ToArray()
        }, cancellationToken);

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (result.IsSuccess)
        {
            TempData["StorageMessage"] = result.Message;
        }
        else
        {
            TempData["StorageError"] = result.Message;
        }

        return RedirectToAction(nameof(Storage));
    }

    [HttpGet("/Storage/Download")]
    public async Task<IActionResult> DownloadStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var file = await _mediator.Send(new DownloadStorageFileQuery
        {
            UserId = currentUserId,
            Scope = scope,
            Name = name
        }, cancellationToken);

        if (file.IsForbidden)
        {
            return Forbid();
        }

        if (file.IsNotFound)
        {
            return NotFound();
        }

        return File(file.Content, file.ContentType, file.DownloadName);
    }

    [HttpGet("/Storage/Preview")]
    public async Task<IActionResult> PreviewStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var file = await _mediator.Send(new DownloadStorageFileQuery
        {
            UserId = currentUserId,
            Scope = scope,
            Name = name
        }, cancellationToken);

        if (file.IsForbidden)
        {
            return Forbid();
        }

        if (file.IsNotFound)
        {
            return NotFound();
        }

        return File(file.Content, file.ContentType);
    }

    [HttpPost("/Storage/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new DeleteStorageFileCommand
        {
            UserId = currentUserId,
            Scope = scope,
            Name = name
        }, cancellationToken);

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (result.IsSuccess)
        {
            TempData["StorageMessage"] = result.Message;
        }
        else
        {
            TempData["StorageError"] = result.Message;
        }

        return RedirectToAction(nameof(Storage));
    }

    [HttpPost("/Storage/Rename")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameStorageFile(string scope, string name, string newName, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new RenameStorageFileCommand
        {
            UserId = currentUserId,
            Scope = scope,
            Name = name,
            NewName = newName
        }, cancellationToken);

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (result.IsSuccess)
        {
            TempData["StorageMessage"] = result.Message;
        }
        else
        {
            TempData["StorageError"] = result.Message;
        }

        return RedirectToAction(nameof(Storage));
    }
}
