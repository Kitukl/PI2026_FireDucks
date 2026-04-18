using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

public class UserProfileController : Controller
{
    private const string UserAvatarContainer = "user-avatars";
    private const int FeedbackSubjectMaxLength = 160;
    private const int FeedbackDescriptionMaxLength = 700;
    private const string FeedbackSubjectPrefix = "Subject: ";

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
        // Жорсткі дефолти
        ViewBag.FullName = "Гість";
        ViewBag.PhotoUrl = Url.Content("~/images/no-photo.png");
        ViewBag.IsNotified = true;
        ViewBag.ReminderOffset = 2u;
        ViewBag.ReminderTimeType = TimeType.Day;

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.Users
                .Include(u => u.Reminder) 
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(_userManager.GetUserId(User)));

            if (user != null)
            {
                ViewBag.FullName = $"{user.Name} {user.Surname}".Trim();
                ViewBag.PhotoUrl = ResolveUserPhotoUrl(user.PhotoUrl);
                ViewBag.IsNotified = user.IsNotified;
        
                if (user.Reminder != null)
                {
                    ViewBag.ReminderOffset = user.Reminder.ReminderOffset;
                    ViewBag.ReminderTimeType = user.Reminder.TimeType;
                }
            }
        }

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
    
    [HttpPost("/UserProfile/UpdateSettings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReminderSettings(bool isNotified, uint offset, TimeType timeType)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.Users
            .Include(u => u.Reminder)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user == null) return Json(new { success = false });

        user.IsNotified = isNotified;
        if (isNotified)
        {
            if (user.Reminder is null)
            {
                user.Reminder = new Reminder();
            }

            user.Reminder.ReminderOffset = offset;
            user.Reminder.TimeType = timeType;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return Json(new { success = false });

        return Json(new { success = true });
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

        var normalizedSubject = (subject ?? string.Empty).Trim();
        var normalizedDescription = (description ?? string.Empty)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Trim();

        if (normalizedSubject.Length > FeedbackSubjectMaxLength)
        {
            normalizedSubject = normalizedSubject[..FeedbackSubjectMaxLength];
        }

        if (string.IsNullOrWhiteSpace(normalizedDescription) && string.IsNullOrWhiteSpace(normalizedSubject))
        {
            TempData["ProfileFeedbackError"] = "Please fill in request details before sending.";
            return RedirectToAction(nameof(UserProfile));
        }

        string fullDescription;
        if (string.IsNullOrWhiteSpace(normalizedSubject))
        {
            fullDescription = normalizedDescription;
        }
        else
        {
            var maxBodyLength = FeedbackDescriptionMaxLength - FeedbackSubjectPrefix.Length - normalizedSubject.Length - 2;
            if (maxBodyLength < 0)
            {
                maxBodyLength = 0;
            }

            if (normalizedDescription.Length > maxBodyLength)
            {
                normalizedDescription = normalizedDescription[..maxBodyLength];
            }

            fullDescription = $"{FeedbackSubjectPrefix}{normalizedSubject}\n\n{normalizedDescription}";
        }

        if (fullDescription.Length > FeedbackDescriptionMaxLength)
        {
            fullDescription = fullDescription[..FeedbackDescriptionMaxLength];
        }

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
