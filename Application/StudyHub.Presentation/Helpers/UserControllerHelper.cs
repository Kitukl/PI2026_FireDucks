using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Users.Commands;

namespace Application.Helpers;

public static class UserControllerHelper
{
    public static async Task<AuthenticationProperties?> CreateMicrosoftChallengePropertiesAsync(
        HttpContext httpContext,
        IUrlHelper url)
    {
        var schemeProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var microsoftScheme = await schemeProvider.GetSchemeAsync(MicrosoftAccountDefaults.AuthenticationScheme);

        if (microsoftScheme == null)
        {
            return null;
        }

        var redirectUrl = url.Action("ExternalCallback", "User");
        return new AuthenticationProperties { RedirectUri = redirectUrl };
    }

    public static async Task<RegisterUserCommand?> CreateExternalRegisterCommandAsync(HttpContext httpContext)
    {
        var result = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return null;
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return new RegisterUserCommand
        {
            Email = email,
            Name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty,
            Surname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty,
            MicrosoftId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            ProviderName = "Microsoft",
            GroupName = "Default",
            SignInAfterRegister = true
        };
    }

    public static Task SignOutExternalSchemeAsync(HttpContext httpContext)
    {
        return httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public static bool IsPath(string? value, string prefix)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    public static Guid? GetCurrentUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    public static async Task<UploadUserProfilePhotoCommand> BuildUploadProfilePhotoCommandAsync(
        IFormFile? photoFile,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        byte[] content = [];
        string fileName = string.Empty;
        string? contentType = null;

        if (photoFile is { Length: > 0 })
        {
            fileName = photoFile.FileName;
            contentType = photoFile.ContentType;

            await using var stream = photoFile.OpenReadStream();
            await using var memory = new MemoryStream();
            await stream.CopyToAsync(memory, cancellationToken);
            content = memory.ToArray();
        }

        return new UploadUserProfilePhotoCommand
        {
            UserId = userId,
            FileName = fileName,
            ContentType = contentType,
            Content = content
        };
    }
}