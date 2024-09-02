using IdentityServer4.Validation;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.Validators;

public class CustomTokenValidator:ICustomTokenValidator
{
    public Task<TokenValidationResult> ValidateAccessTokenAsync(TokenValidationResult result)
    {
        throw new NotImplementedException();
    }

    public Task<TokenValidationResult> ValidateIdentityTokenAsync(TokenValidationResult result)
    {
        throw new NotImplementedException();
    }
}