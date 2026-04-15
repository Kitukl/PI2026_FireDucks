using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using StudyHub.Domain.Entities;

namespace Application.Security;

public class UserRolesClaimsTransformation : IClaimsTransformation
{
    private readonly UserManager<User> _userManager;

    public UserRolesClaimsTransformation(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out _))
        {
            return principal;
        }

        var identity = principal.Identities.FirstOrDefault(i => i.IsAuthenticated);
        if (identity == null)
        {
            return principal;
        }

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            return principal;
        }

        var existingRoleClaims = identity.FindAll(identity.RoleClaimType).ToList();
        foreach (var roleClaim in existingRoleClaims)
        {
            identity.RemoveClaim(roleClaim);
        }

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            identity.AddClaim(new Claim(identity.RoleClaimType, role));
        }

        return principal;
    }
}
