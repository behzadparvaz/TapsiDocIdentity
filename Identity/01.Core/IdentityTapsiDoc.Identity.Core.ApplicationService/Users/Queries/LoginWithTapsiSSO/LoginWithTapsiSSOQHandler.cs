using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using MediatR;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;

public class LoginWithTapsiSSOQHandler : IRequestHandler<LoginWithTapsiSSOQuery, RegisterSummery>
{

    ITapsiSsoService _tapsiSsoService;

    public LoginWithTapsiSSOQHandler(ITapsiSsoService tapsiSsoService)
    {
        _tapsiSsoService = tapsiSsoService;
    }

    public async Task<RegisterSummery> Handle(LoginWithTapsiSSOQuery request,
        CancellationToken cancellationToken)
    {
        var tokenRes = await _tapsiSsoService.GetToken(request.Code);
        var user = tokenRes.User;
        Thread.Sleep(10000);
        return new RegisterSummery
        {
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
            IsActive = true,
            IsRegister = true,
            PhoneNumber = user.PhoneNumber,
            StatusCode = 200,
            Message = "succeeded",
            Token = tokenRes.Token
        };
    }
}