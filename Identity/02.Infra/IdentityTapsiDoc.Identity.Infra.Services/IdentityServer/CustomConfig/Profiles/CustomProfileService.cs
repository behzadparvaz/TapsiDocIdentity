using System.Security.Claims;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.Profiles;

public class CustomProfileService : ProfileService<User>
{
    private UserManager<User> _userManager;

    public CustomProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsFactory) : base(
        userManager, claimsFactory)
    {
        _userManager = userManager;
    }

    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtClaimTypes.PhoneNumber, user.PhoneNumber)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(JwtClaimTypes.Role, role));
        }

        context.IssuedClaims.AddRange(claims);
    }
}