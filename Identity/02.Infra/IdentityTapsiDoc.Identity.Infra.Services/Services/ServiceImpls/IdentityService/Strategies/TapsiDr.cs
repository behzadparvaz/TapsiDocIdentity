using IdentityModel.Client;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.GrantTypes;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Strategies;

public class TapsiDr : IIdentityServiceStrategy
{
    public AuthenticationType AuthenticationType => AuthenticationType.TapsiDr;

    private UserManager<User> _userManager;
    private IHttpClientFactory _httpClientFactory;
    private IConfiguration _configuration;

    public TapsiDr(UserManager<User> userManager, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<GetTokenOutput> GetToken(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("USER NOT FOUND");
        }

        var httpClient = _httpClientFactory.CreateClient();

        var baseUrl = _configuration["IdentityServerConf:BaseUrl"] ??
                      throw new NullReferenceException("IDENTITY SERVER URL NOTFOUND");

        var clientId = _configuration["IdentityServerConf:ClientId"] ??
                       throw new NullReferenceException("IDENTITY SERVER CLIENT ID NOTFOUND");

        var clientSecret = _configuration["IdentityServerConf:ClientSecret"] ??
                           throw new NullReferenceException("IDENTITY SERVER CLIENT SECRET NOTFOUND");

        var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(baseUrl);
        if (discoveryDocument.IsError)
        {
            throw new ArgumentException("IDENTITY SERVER GET DISCOVERY ERR");
        }
        var tokenResponse = await httpClient.RequestTokenAsync(
            new ClientCredentialsTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = CustomGrantType.SecurityStamp,
                Parameters =
                {
                    { "SecurityStamp", user.SecurityStamp },
                    { "UserName", user.UserName },
                }
            });
        if (tokenResponse.IsError)
        {
            throw new ArgumentException("IDENTITY SERVER GET TOKEN ERR");
        }

        return new GetTokenOutput()
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            IdToken = tokenResponse.IdentityToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }

    public Task<GetUserInfoOutput> GetUserInfo(string accessToken)
    {
        throw new NotImplementedException();
    }
}