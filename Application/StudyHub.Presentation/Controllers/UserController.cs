using MediatR;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Helpers;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

[Route("user")]
public class UserController : Controller
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "UserPlatform");
        }

        return View("Login");
    }

    [AllowAnonymous]
    [HttpGet("login-microsoft")]
    public async Task<IActionResult> LoginMicrosoft()
    {
        var properties = await UserControllerHelper.CreateMicrosoftChallengePropertiesAsync(HttpContext, Url);
        if (properties == null)
        {
            return RedirectToAction("Index", "UserPlatform");
        }

        return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var command = await UserControllerHelper.CreateExternalRegisterCommandAsync(HttpContext);
        if (command == null)
        {
            return RedirectToAction("Index", "UserPlatform");
        }

        await _mediator.Send(command);
        await UserControllerHelper.SignOutExternalSchemeAsync(HttpContext);

        return RedirectToAction("Index", "UserPlatform");
    }

    [HttpGet("access-denied")]
    public IActionResult AccessDenied(string? returnUrl)
    {
        returnUrl ??= Request.Query["ReturnUrl"].FirstOrDefault();

        if (UserControllerHelper.IsPath(returnUrl, "/Admin"))
        {
            if (User.IsInRole(nameof(Role.Admin)))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "UserPlatform");
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new SignOutUserCommand());
        return RedirectToAction("Index", "UserPlatform");
    }

    [Authorize(Roles = nameof(Role.Leader))]
    [HttpPut]
    public async Task<IActionResult> AddUserToGroup(AddUserToGroupCommand request)
    {
        await _mediator.Send(request);
        return View();
    }

    [Authorize(Roles = nameof(Role.Leader))]
    [HttpPut]
    public async Task<IActionResult> RemoveUserFromGroup(RemoveUserFromGroupCommand request)
    {
        await _mediator.Send(request);
        return View();
    }

    [HttpGet("/myprofile")]
    [HttpGet("/UserProfile")]
    public async Task<IActionResult> UserProfile()
    {
        var parsedUserId = UserControllerHelper.GetCurrentUserId(User);

        var data = await _mediator.Send(new GetUserProfilePageQuery
        {
            UserId = parsedUserId
        });

        return View("~/Views/UserPlatform/UserProfile/UserProfile.cshtml", data);
    }

    [HttpGet("/UserProfile/ViewRequest/{feedbackId?}")]
    public async Task<IActionResult> ViewRequest(string? feedbackId)
    {
        var parsedUserId = UserControllerHelper.GetCurrentUserId(User);

        var data = await _mediator.Send(new GetUserProfilePageQuery
        {
            UserId = parsedUserId,
            FeedbackId = feedbackId,
            OpenModal = true
        });

        return View("~/Views/UserPlatform/UserProfile/UserProfile.cshtml", data);
    }

    [HttpPost("/UserProfile/Photo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile? photoFile, CancellationToken cancellationToken)
    {
        var parsedUserId = UserControllerHelper.GetCurrentUserId(User);
        var command = await UserControllerHelper.BuildUploadProfilePhotoCommandAsync(photoFile, parsedUserId, cancellationToken);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            TempData["ProfileFeedbackError"] = result.ErrorMessage;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["ProfileFeedbackSuccess"] = result.SuccessMessage;
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpGet("/UserProfile/PhotoFile")]
    public async Task<IActionResult> UserProfilePhoto([FromQuery] string path, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadUserProfilePhotoQuery
        {
            Path = path
        }, cancellationToken);

        if (result.IsNotFound || result.Content == null)
        {
            return NotFound();
        }

        return File(result.Content, result.ContentType);
    }

    [HttpPost("/UserProfile/UpdateSettings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReminderSettings(bool isNotified, uint offset, TimeType timeType)
    {
        var parsedUserId = UserControllerHelper.GetCurrentUserId(User);

        var result = await _mediator.Send(new UpdateUserReminderSettingsCommand
        {
            UserId = parsedUserId,
            IsNotified = isNotified,
            Offset = offset,
            TimeType = timeType
        });

        return Json(new { success = result.IsSuccess });
    }

    [HttpPost("/UserProfile/SendRequest")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfileSendRequest(
        FeedbackType feedbackType,
        Category category,
        string? subject,
        string? description)
    {
        var parsedUserId = UserControllerHelper.GetCurrentUserId(User);

        var result = await _mediator.Send(new CreateUserProfileRequestCommand
        {
            UserId = parsedUserId,
            FeedbackType = feedbackType,
            Category = category,
            Subject = subject,
            Description = description
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            TempData["ProfileFeedbackError"] = result.ErrorMessage;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["ProfileFeedbackSuccess"] = result.SuccessMessage;
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpPost("/UserProfile/AddRequestComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRequestComment(Guid feedbackId, string? description)
    {
        var result = await _mediator.Send(new AddRequestCommentCommand
        {
            CurrentUserId = UserControllerHelper.GetCurrentUserId(User),
            FeedbackId = feedbackId,
            Description = description,
            IsAdmin = User.IsInRole(nameof(Role.Admin))
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (result.IsNotFound)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(ViewRequest), new { feedbackId });
    }

    [HttpPost("/UserProfile/DeleteRequestComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRequestComment(Guid feedbackId, Guid commentId)
    {
        var result = await _mediator.Send(new DeleteRequestCommentCommand
        {
            CurrentUserId = UserControllerHelper.GetCurrentUserId(User),
            FeedbackId = feedbackId,
            CommentId = commentId,
            IsAdmin = User.IsInRole(nameof(Role.Admin))
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (result.IsNotFound)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(ViewRequest), new { feedbackId });
    }

}