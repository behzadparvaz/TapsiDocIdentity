using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.DTOs;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using MediatR;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;

public class LoginWithTapsiSSOQHandler : IRequestHandler<LoginWithTapsiSSOQuery, RegisterSummery>
{
    private IIdentityService _identityService;

    public LoginWithTapsiSSOQHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RegisterSummery> Handle(LoginWithTapsiSSOQuery request,
        CancellationToken cancellationToken)
    {
        var tokenRes = await _identityService.Login(request.Code, AuthenticationType.TapsiSso);
        var res = await _identityService.GetUserInfo(tokenRes.AccessToken);
        return new RegisterSummery
        {
            HasPassword = false,
            IsActive = true,
            IsRegister = true,
            PhoneNumber = res,
            StatusCode = 200,
            Message = "succeeded",
            Token = tokenRes.AccessToken
        };
        return default;
    }
}