using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

public class UserProfileController : Controller
{
    private const string UserAvatarContainer = "user-avatars";

    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;
    private readonly StudyHub.Core.Storage.Interfaces.IBlobService _blobService;

    public UserProfileController(
        UserManager<User> userManager,
        IMediator mediator,
        StudyHub.Core.Storage.Interfaces.IBlobService blobService)
    {
        _userManager = userManager;
        _mediator = mediator;
        _blobService = blobService;
    }

    [HttpGet("/myprofile")]
    [HttpGet("/UserProfile")]
    public async Task<IActionResult> UserProfile()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.FullName = $"{user.Name} {user.Surname}".Trim();
                ViewBag.PhotoUrl = ResolveUserPhotoUrl(user.PhotoUrl);
            }
        }

        ViewBag.FullName ??= "Гість";
        ViewBag.PhotoUrl ??= Url.Content("~/images/no-photo.png");

        return View("~/Views/Home/UserProfile/UserProfile.cshtml");
    }

    [HttpPost("/UserProfile/Photo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile? photoFile, CancellationToken cancellationToken)
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

        if (photoFile is not { Length: > 0 })
        {
            TempData["ProfileFeedbackError"] = "Please select an image first.";
            return RedirectToAction(nameof(UserProfile));
        }

        var safeFileName = Path.GetFileName(photoFile.FileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            safeFileName = $"avatar-{Guid.NewGuid():N}.bin";
        }

        var blobName = $"{user.Id:D}/{Guid.NewGuid():N}-{safeFileName}";
        await using var stream = photoFile.OpenReadStream();

        await _blobService.UploadFileAsync(
            UserAvatarContainer,
            blobName,
            stream,
            photoFile.ContentType,
            cancellationToken);

        if (TryExtractAvatarBlobName(user.PhotoUrl, out var oldBlobName))
        {
            await _blobService.DeleteFileAsync(UserAvatarContainer, oldBlobName, cancellationToken);
        }

        user.PhotoUrl = $"{UserAvatarContainer}/{blobName}";
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            TempData["ProfileFeedbackError"] = "Failed to save profile photo.";
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["ProfileFeedbackSuccess"] = "Profile photo updated.";
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpGet("/UserProfile/PhotoFile")]
    public async Task<IActionResult> UserProfilePhoto([FromQuery] string path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return NotFound();
        }

        var marker = UserAvatarContainer + "/";
        var blobName = path.StartsWith(marker, StringComparison.OrdinalIgnoreCase)
            ? path[marker.Length..]
            : path;

        var fileStream = await _blobService.GetFileAsync(UserAvatarContainer, blobName, cancellationToken);
        if (fileStream == null)
        {
            return NotFound();
        }

        return File(fileStream, GetImageContentType(blobName));
    }

    [HttpPost("/UserProfile/SendRequest")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfileSendRequest(
        FeedbackType feedbackType,
        Category category,
        string? subject,
        string? description)
    {
        var user = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(subject))
        {
            TempData["ProfileFeedbackError"] = "Please fill in request details before sending.";
            return RedirectToAction(nameof(UserProfile));
        }

        var fullDescription = string.IsNullOrWhiteSpace(subject)
            ? (description ?? string.Empty).Trim()
            : $"Subject: {subject.Trim()}\n\n{(description ?? string.Empty).Trim()}";

        await _mediator.Send(new CreateFeedbackCommand
        {
            UserId = user.Id,
            FeedbackType = feedbackType,
            Category = category,
            Description = fullDescription,
            Status = Status.ToDo
        });

        TempData["ProfileFeedbackSuccess"] = "Request sent.";
        return RedirectToAction(nameof(UserProfile));
    }

    private string ResolveUserPhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Url.Content("~/images/no-photo.png");
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            return Url.Action(nameof(UserProfilePhoto), new { path = photoUrl })
                   ?? Url.Content("~/images/no-photo.png");
        }

        return photoUrl;
    }

    private static bool TryExtractAvatarBlobName(string? photoUrl, out string blobName)
    {
        blobName = string.Empty;
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return false;
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            blobName = photoUrl[marker.Length..];
            return !string.IsNullOrWhiteSpace(blobName);
        }

        if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var absolutePath = uri.AbsolutePath.Replace('\\', '/');
        var markerIndex = absolutePath.IndexOf("/" + marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return false;
        }

        var start = markerIndex + marker.Length + 1;
        if (start >= absolutePath.Length)
        {
            return false;
        }

        blobName = absolutePath[start..];
        return !string.IsNullOrWhiteSpace(blobName);
    }

    private static string GetImageContentType(string blobName)
    {
        var extension = Path.GetExtension(blobName);
        return extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}
