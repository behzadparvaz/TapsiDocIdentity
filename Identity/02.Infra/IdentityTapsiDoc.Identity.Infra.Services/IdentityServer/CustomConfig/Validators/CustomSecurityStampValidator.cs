using IdentityServer4.Validation;
using IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.GrantTypes;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.Validators;

public class CustomSecurityStampValidator : IExtensionGrantValidator
{
    public string GrantType => "test";

    public Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        context.Result = new GrantValidationResult("user.Id", "test");
        return Task.CompletedTask;
    }
}