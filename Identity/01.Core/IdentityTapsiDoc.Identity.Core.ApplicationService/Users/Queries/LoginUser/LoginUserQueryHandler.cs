using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
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
        IIdentityService _identityService;

        public LoginUserQueryHandler(UserManager<User> userManager, SignInManager<User> signInManager, IIdentityService identityService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            _identityService = identityService;
        }

        public async Task<RegisterSummery> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.LoginByPasswordAsync(request.PhoneNumber, request.Password);

            var token = await _identityService.GenerateTokenAsync(user.PhoneNumber);

            return new RegisterSummery
            {
                HasPassword = false,
                IsActive = true,
                IsRegister = true,
                PhoneNumber = request.PhoneNumber,
                StatusCode = 200,
                Message = "succeeded",
                Token = token
            };
        }
    }
}
