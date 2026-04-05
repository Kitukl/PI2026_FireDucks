using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StudyHub.Domain.Entities;

namespace Application.Security;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole<Guid>>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Ensure only Email claim is stored (along with default ones like NameIdentifier and role)
        var emailClaim = identity.FindFirst(ClaimTypes.Email);
        if (emailClaim == null && !string.IsNullOrEmpty(user.Email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        }

        return identity;
    }
}
