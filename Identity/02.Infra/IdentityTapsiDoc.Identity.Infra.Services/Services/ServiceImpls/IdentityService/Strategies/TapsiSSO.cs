using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.GrantTypes;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Strategies;

public class TapsiSSO : IIdentityServiceStrategy
{
    public AuthenticationType AuthenticationType => AuthenticationType.TapsiSso;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    #region Fields

    private readonly string _redirectUri;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _baseUri;
    private readonly string _tokenUri;
    private readonly string _userInfoUri;

    #endregion

    public TapsiSSO(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        _redirectUri = _configuration["IdentityServerConf:TapsiSSO:RedirectUri"]
                       ?? throw new NullReferenceException("REDIRECT URI NOTFOUND");
        _clientId = _configuration["IdentityServerConf:TapsiSSO:ClientId"]
                    ?? throw new NullReferenceException("CLIENT ID NOT FOUND");
        _clientSecret = _configuration["IdentityServerConf:TapsiSSO:ClientSecret"]
                        ?? throw new NullReferenceException("CLIENT SECRET NOT FOUND");
        _baseUri = _configuration["IdentityServerConf:TapsiSSO:BaseUri"]
                   ?? throw new NullReferenceException("TAPSI SSO BASE URI NOT FOUND");
        _tokenUri = _configuration["IdentityServerConf:TapsiSSO:TokenUri"]
                    ?? throw new NullReferenceException("TAPSI SSO TOKEN URI NOT FOUND");
        _userInfoUri = _configuration["IdentityServerConf:TapsiSSO:UserInfoUri"]
                    ?? throw new NullReferenceException("TAPSI SSO TOKEN URI NOT FOUND");
    }

    public async Task<GetTokenOutput> GetToken(string userName)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.SetBasicAuthentication(_clientId, _clientSecret);
        var tokenResponse = await httpClient.RequestTokenAsync(
            new ClientCredentialsTokenRequest()
            {
                Method = HttpMethod.Post,
                Address = _baseUri + _tokenUri,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                GrantType = GrantType.AuthorizationCode,
                Parameters =
                {
                    { "code", userName },
                    { "redirect_uri", _redirectUri },
                    { "custom_claims", ""}, //TODO: SET CLAIMS - NOT TEST !
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

    public async Task<GetUserInfoOutput> GetUserInfo(string accessToken)
    {
        var client = new RestClient();
        var request = new RestRequest(_baseUri + _userInfoUri, Method.Get);
        request.AddHeader("Authorization", "Bearer " + accessToken);
        RestResponse response = await client.ExecuteAsync(request);
        var jobject = JObject.Parse(response.Content);
        var x = jobject.Value<string>("phone_number");

        return new GetUserInfoOutput()
        {
            PhoneNumber = x
        };
    }
}