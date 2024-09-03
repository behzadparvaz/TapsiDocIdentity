
using Azure.Core;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;

public class LoginWithTapsiSSOQHandler : IRequestHandler<LoginWithTapsiSSOQuery, RegisterSummery>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserManager<User> _userManager;


    public LoginWithTapsiSSOQHandler(IHttpClientFactory httpClientFactory, UserManager<User> userManager)
    {
        _httpClientFactory = httpClientFactory;
        _userManager = userManager;
    }

    public async Task<RegisterSummery> Handle(LoginWithTapsiSSOQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = "doctor.tapsi";
        var clientSecret = "aa4c844e-f354-4920-b1ea-db04a0cbb671";
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.SetBasicAuthentication(clientId, clientSecret);
        var tokenResponse = await httpClient.RequestTokenAsync(
            new ClientCredentialsTokenRequest()
            {
                Method = HttpMethod.Post,
                Address = "https://accounts-api.tapsi.ir/api/v1/sso-user/oidc/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = GrantType.AuthorizationCode,
                Parameters =
                {
                    { "code", request.Code },
                    { "redirect_uri", "https://tapsi.doctor/app" },
                    //{ "custom_claims", ""}, //TODO: SET CLAIMS - NOT TEST !
                }
            });


        if (tokenResponse.IsError)
        {
            throw new ArgumentException("IDENTITY SERVER GET TOKEN ERR");
        }


        var tokenReader = new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.IdentityToken);
        var claims = tokenReader.Claims;

        var globalUserId = claims.Where(z => z.Type == "global_user_id")
                .Select(z => z.Value)
                .FirstOrDefault("");

        //+989398200778
        var phoneNumber = claims.Where(z => z.Type == "phone_number")
                .Select(z => z.Value)
                .FirstOrDefault("").Replace("+98", "0");

        var expireTime = claims.Where(z => z.Type == "exp")
                .Select(z => z.Value)
                .FirstOrDefault("");

        var token = new JwtTokenBuilder()
                          .AddSecurityKey(ApplicationTokens.Tokens["TapsiDocApp"])
                          .AddSubject(phoneNumber)
                          .AddIssuer("Tapsi")
                          .AddAudience("TapsiDocApp")
                          .AddClaim(ClaimTypes.Name, phoneNumber)
                          .AddClaim(ClaimTypes.Role, "User")
                          .AddClaim(ClaimTypes.MobilePhone, phoneNumber)
                          .AddExpiry(TimeSpan.FromSeconds(int.Parse(expireTime)).Minutes) // 30 Min
                          .Build();

        var user = _userManager.Users.FirstOrDefault(z => z.PhoneNumber == phoneNumber);
        if (user == null)
        {
            //TODO:Create User
        }
        user.TapsiUserId = globalUserId;
        await _userManager.UpdateAsync(user);
        return new RegisterSummery
        {
            HasPassword = false,
            IsActive = true,
            IsRegister = true,
            PhoneNumber = phoneNumber,
            StatusCode = 200,
            Message = "succeeded",
            Token = token.Value
        };
    }
}