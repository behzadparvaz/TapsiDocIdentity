using IdentityModel.Client;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.GrantTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IdentityTapsiDoc.Identity.Infra.Services;

public class IdentityService : IIdentityService
{
    private UserManager<User> _userManager;
    private IHttpClientFactory _httpClientFactory;
    private IConfiguration _configuration;

    public IdentityService(UserManager<User> userManager, IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task GetToken(string userName, AuthenticationType authType)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("کاربر یافت نشد");
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
                    { "AuthType", authType.ToString() }
                }
            });
        if (tokenResponse.IsError)
        {
            throw new ArgumentException("IDENTITY SERVER GET TOKEN ERR");
        }
    }
}