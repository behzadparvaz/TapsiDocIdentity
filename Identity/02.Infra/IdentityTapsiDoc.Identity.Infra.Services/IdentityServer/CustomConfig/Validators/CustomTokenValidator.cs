using IdentityServer4.Validation;

namespace IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.Validators;

public class CustomTokenValidator : ITokenValidator
{
    public Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null)
    {
        throw new NotImplementedException();
    }

    public Task<TokenValidationResult> ValidateIdentityTokenAsync(string token, string clientId = null, bool validateLifetime = true)
    {
        throw new NotImplementedException();
    }
}