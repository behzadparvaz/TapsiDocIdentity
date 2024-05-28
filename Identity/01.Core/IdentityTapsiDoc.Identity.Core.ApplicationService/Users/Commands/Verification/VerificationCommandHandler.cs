using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification
{
    public class VerificationCommandHandler : IRequestHandler<VerificationCommand, RegisterSummery>
    {
        private readonly IUserQueryRepository query;
        private readonly UserManager<User> userManager;

        public VerificationCommandHandler(IUserQueryRepository query, UserManager<User> userManager)
        {
            this.query = query;
            this.userManager = userManager;
        }

        public async Task<RegisterSummery> Handle(VerificationCommand request, CancellationToken cancellationToken)
        {
            var result = await this.query.CheckCode(request.PhoneNumber, request.Code);
            if (!result)
                throw new ArgumentException("the code not valid");

            var findUser = await this.userManager.FindByNameAsync(request.PhoneNumber);
            if (findUser == null)
            {
                User user = new()
                {
                    UserName = request.PhoneNumber,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = string.Empty,
                    LastName = string.Empty
                };
                var resultUser = userManager.CreateAsync(user).Result;
                if (resultUser.Succeeded)
                {
                    IEnumerable<Claim> claims = new Claim[] {
                             new Claim(ClaimTypes.Name , request.PhoneNumber , ClaimValueTypes.String ),
                             new Claim(ClaimTypes.Role , "User" , ClaimValueTypes.String ),
                             new Claim(ClaimTypes.MobilePhone , user.PhoneNumber , ClaimValueTypes.String )
                        };
                    await userManager.AddClaimsAsync(user, claims);
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

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
    }
}
