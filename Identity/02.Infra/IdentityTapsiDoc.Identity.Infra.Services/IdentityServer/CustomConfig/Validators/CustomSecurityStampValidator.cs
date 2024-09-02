using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.GrantTypes;
using Microsoft.AspNetCore.Identity;

namespace IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.Validators;

public class CustomSecurityStampValidator : IExtensionGrantValidator
{
    public string GrantType => CustomGrantType.SecurityStamp;

    private UserManager<User> _userManager;
    private IHttpClientFactory _httpClientFactory;

    public CustomSecurityStampValidator(UserManager<User> userManager, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var securityStamp = context?.Request?.Raw?["SecurityStamp"] ?? "";
        var phoneNumber = context?.Request?.Raw?["UserName"] ?? "";

        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(securityStamp))
        {
            context.Result =
                new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    "PHONE NUMBER OR STAMP INVALID");
            return;
        }

        var user = await _userManager.FindByNameAsync(phoneNumber);
        if (user == null || user.SecurityStamp != securityStamp)
        {
            context.Result =
                new GrantValidationResult(TokenRequestErrors.UnauthorizedClient,
                    "USER NOT FOUND");
            return;
        }

        context.Result = new GrantValidationResult(user.Id, CustomGrantType.SecurityStamp);
    }
}