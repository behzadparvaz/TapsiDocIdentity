using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginByOtp
{
    public class LoginByOtpQueryHandler : IRequestHandler<LoginByOtpQuery, RegisterSummery>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserCommandRepository command;
        IIdentityService _identityService;

        public LoginByOtpQueryHandler(UserManager<User> userManager, IUserCommandRepository command, IIdentityService identityService)
        {
            this._userManager = userManager;
            this.command = command;
            _identityService = identityService;
        }

        public async Task<RegisterSummery> Handle(LoginByOtpQuery request, CancellationToken cancellationToken)
        {
            //TODO: change request - add OTP 
            var user = await _identityService.LoginByOTPAsync(request.PhoneNumber, "");

            var token = await _identityService.GenerateTokenAsync(user.PhoneNumber);

            return new RegisterSummery
            {
                HasPassword = false,
                IsActive = true,
                IsRegister = true,
                PhoneNumber = user.PhoneNumber,
                StatusCode = 200,
                Message = "succeeded",
                Token = token
            };
        }
    }
}
