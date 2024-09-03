using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, RegisterSummery>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginUserQueryHandler(UserManager<User> userManager , SignInManager<User> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public async Task<RegisterSummery> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            await _signInManager.SignOutAsync();
            var result =await _signInManager.PasswordSignInAsync(user, request.Password, true, true);
            if (result.Succeeded)
            {
                var token = new JwtTokenBuilder()
                            .AddSecurityKey(ApplicationTokens.Tokens["TapsiDocApp"])
                            .AddSubject(request.PhoneNumber)
                            .AddIssuer("Tapsi")
                            .AddAudience("TapsiDocApp")
                            .AddClaim(ClaimTypes.Name, request.PhoneNumber)
                            .AddClaim(ClaimTypes.Role, "User")
                            .AddClaim(ClaimTypes.MobilePhone, request.PhoneNumber)
                            .AddExpiry(24 * 60 * 3650)
                            .Build();

                return new RegisterSummery
                {
                    HasPassword = false,
                    IsActive = true,
                    IsRegister = true,
                    PhoneNumber = request.PhoneNumber,
                    StatusCode = 200,
                    Message = "succeeded",
                    Token = token.Value
                };
            }
            else
                throw new ArgumentException("نام کاربری یا کلمه عبور اشتباه است ");
        }
    }
}
