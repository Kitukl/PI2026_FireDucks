using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Users.Commands;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

[Route("user")]
public class UserController : Controller
{
    private readonly ISender _mediator;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    // Додаємо менеджери Identity через конструктор
    public UserController(
        ISender mediator, 
        UserManager<User> userManager, 
        SignInManager<User> signInManager)
    {
        _mediator = mediator;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet("login-microsoft")]
    public IActionResult LoginMicrosoft()
    {
        var redirectUrl = Url.Action("ExternalCallback", "User"); 
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        
        if (!result.Succeeded || result.Principal == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

        var command = new RegisterUserCommand
        {
            Email = email,
            Name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
            Surname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            MicrosoftId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
            ProviderName = "Microsoft",
            GroupName = "Default"
        };

        await _mediator.Send(command);

        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
        }
        
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
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
}