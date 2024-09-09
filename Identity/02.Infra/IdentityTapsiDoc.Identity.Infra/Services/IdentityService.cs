using IdentityModel;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Infra.Services
{
    public class IdentityService : IIdentityService
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;

        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> RegisterAsync(string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user != null)
                return user;

            user = new User()
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                FirstName = "",
                LastName = ""
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
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.UserName ?? ""));
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
        public async Task<string> GenerateTokenAsync(
            string phoneNumber,
            int expireInMinute = 60,
            string issuer = "")
        {

            var tokenBuilder = new JwtTokenBuilder()
            .AddSecurityKey(ApplicationTokens.Tokens["TapsiDocApp"])
            .AddIssuer(issuer)
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
