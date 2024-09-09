using Azure.Core;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services.TapsiSso;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityTapsiDoc.Identity.Infra.Services
{
    public class TapsiSsoService : ITapsiSsoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IIdentityService _identityService;

        public TapsiSsoService(IHttpClientFactory httpClientFactory, UserManager<User> userManager, IConfiguration configuration, IIdentityService identityService)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _configuration = configuration;
            _identityService = identityService;
        }

        public async Task<GetTokenOutput> GetToken(string code)
        {

            var clientId = _configuration["ServicesConfig:TapsiSso:ClientId"] ??
                throw new ArgumentNullException("CLIENT ID NOT FOUND");

            var clientSecret = _configuration["ServicesConfig:TapsiSso:ClientSecret"] ??
                throw new ArgumentNullException("CLIENT SECRET NOT FOUND");

            var tokenUrl = _configuration["ServicesConfig:TapsiSso:TokenUrl"] ??
                throw new ArgumentNullException("TOKEN URL NOT FOUND");

            var redirectUrl = _configuration["ServicesConfig:TapsiSso:RedirectUrl"] ??
                throw new ArgumentNullException("REDIRECT URL NOT FOUND");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.SetBasicAuthentication(clientId, clientSecret);
            var tokenResponse = await httpClient.RequestTokenAsync(
                new ClientCredentialsTokenRequest()
                {
                    Method = HttpMethod.Post,
                    Address = tokenUrl,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    GrantType = GrantType.AuthorizationCode,
                    Parameters =
                    {
                        { "code", code },
                        { "redirect_uri", redirectUrl },
                    }
                });

            if (tokenResponse.IsError)
            {
                throw new ArgumentException("IDENTITY SERVER GET TOKEN ERR");
            }

            var tapsiDrToken = await MergeTokenTapsiToTapsiDr(tokenResponse.IdentityToken);
            return new GetTokenOutput()
            {
                Token = tapsiDrToken.Item1,
                User = tapsiDrToken.Item2
            };

        }
        private async Task<(string, User)> MergeTokenTapsiToTapsiDr(string tapsiToken)
        {
            if (string.IsNullOrEmpty(tapsiToken))
                throw new ArgumentException("TOKEN IS NULL OR EMPTY");

            var tokenIssuer = _configuration["ServicesConfig:TapsiSso:JwtConfig:Issuer"] ??
                throw new ArgumentException("TOKEN ISSUER NOT FOUND");

            var tokenReader = new JwtSecurityTokenHandler().ReadJwtToken(tapsiToken);
            var claims = tokenReader.Claims;

            var globalUserId = claims.Where(z => z.Type == "global_user_id")
                    .Select(z => z.Value)
                    .FirstOrDefault("");

            var phoneNumber = claims.Where(z => z.Type == "phone_number")
                    .Select(z => z.Value)
                    .FirstOrDefault("").Replace("+98", "0");

            var expireTime = claims.Where(z => z.Type == "exp")
                    .Select(z => z.Value)
                    .FirstOrDefault("");

            var firstName = claims.Where(z => z.Type == "display_name")
                    .Select(z => z.Value)
                    .FirstOrDefault("");

            var lastName = claims.Where(z => z.Type == "display_last_name")
                   .Select(z => z.Value)
                   .FirstOrDefault("");

            var user = await _userManager.FindByNameAsync(phoneNumber);
            bool isNeedUpdate = false;
            if (user == null)
            {
                user = await _identityService.RegisterAsync(phoneNumber);

                user.TapsiUserId = globalUserId;
                isNeedUpdate = true;
            }
            if (string.IsNullOrEmpty(firstName))
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                isNeedUpdate = true;
            }
            if (isNeedUpdate)
                await _userManager.UpdateAsync(user);


            var token = await _identityService.GenerateTokenAsync(
                phoneNumber,
                expireInMinute: TimeSpan.FromSeconds(int.Parse(expireTime)).Minutes,
                issuer: tokenIssuer);

            return (token, user);
        }
    }
}
