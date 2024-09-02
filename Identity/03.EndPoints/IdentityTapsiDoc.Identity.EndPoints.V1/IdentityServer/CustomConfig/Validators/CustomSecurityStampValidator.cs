using IdentityServer4.Validation;
using IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.GrantTypes;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.Validators;

public class CustomCodeValidator : IExtensionGrantValidator
{
    public string GrantType => CustomGrantType.SecurityStamp;

    public Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        throw new NotImplementedException();
    }
}