using Azure.Core;
using IdentityModel;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityTapsiDoc.Identity.Infra.Services
{
    public class IdentityService : IIdentityService
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;
        IUserQueryRepository _userQueryRepository;

        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager, IUserQueryRepository userQueryRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userQueryRepository = userQueryRepository;
        }
        public async Task<User> LoginByOTPAsync(string phoneNumber, string OTP)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user == null)
                throw new NullReferenceException("USER NOT FOUND");

            var otpIsValid = await _userQueryRepository.CheckCode(phoneNumber, OTP);
            if (!otpIsValid)
                throw new Exception("OTP INVALID");
            return user;

        }
        public async Task<User> LoginByPasswordAsync(string phoneNumber, string password)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user == null)
                throw new NullReferenceException("USER NOT FOUND");

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);
            if (!signInResult.Succeeded)
                throw new Exception("PASSWORD IS INVALID");

            return user;

        }
        public async Task<User> RegisterAsync(string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user != null)
                return user;

            user = new User()
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber
            };

            var createUserRes = await _userManager.CreateAsync(user);
            if (!createUserRes.Succeeded)
                throw new Exception("CREATE USER ERR");

            await SetRolesAsync(user, Roles.User);

            return user;
        }
        public async Task SetRolesAsync(User user, params Roles[] roles)
        {
            var claims = new List<Claim>();
            var strRoles = new List<string>();
            foreach (var role in roles)
            {
                var tmpStrRole = role.ToString();
                claims.Add(new Claim(JwtClaimTypes.Role, tmpStrRole));
                claims.Add(new Claim(JwtClaimTypes.Name, user.UserName ?? ""));
                claims.Add(new Claim(JwtClaimTypes.Subject, user.UserName ?? ""));
                strRoles.Add(tmpStrRole);
            }

            var setClaimRes = await _userManager.AddClaimsAsync(user, claims);
            if (!setClaimRes.Succeeded)
                throw new Exception("SET CLAIMS ERR");

            var setRoleRes = await _userManager.AddToRolesAsync(user, strRoles);
            if (!setRoleRes.Succeeded)
                throw new Exception("SET ROLES ERR");
        }

        public async Task<string> GenerateTokenAsync(string phoneNumber, int expireInMinute = 60)
        {

            var tokenBuilder = new JwtTokenBuilder()
            .AddSecurityKey(ApplicationTokens.Tokens["TapsiDocApp"])
            .AddIssuer("Tapsi")
            .AddExpiry(expireInMinute);

            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user == null)
                throw new NullReferenceException("USER NOT FOUND");

            var claims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
            {
                tokenBuilder.AddClaim(claim.Type, claim.Value);
            }
            var jwtToken = tokenBuilder.Build();
            if (string.IsNullOrEmpty(jwtToken.Value))
                throw new NullReferenceException("TOKEN GENERATE ERR");

            return jwtToken.Value;
        }
    }
}
