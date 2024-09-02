using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterSummery>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserCommandRepository command;
        IIdentityService _identityService;

        public RegisterUserCommandHandler(UserManager<User> userManager, IUserCommandRepository command, IIdentityService identityService)
        {
            this._userManager = userManager;
            this.command = command;
            _identityService = identityService;
        }

        public async Task<RegisterSummery> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            var user = await _identityService.RegisterAsync(request.PhoneNumber);
            await command.SendOtpCode(request.PhoneNumber);

            return new RegisterSummery
            {
                HasPassword = user.PasswordHash != null,
                IsActive = true,
                IsRegister = false, //TODO: Why is it needed?
                PhoneNumber = request.PhoneNumber,
                StatusCode = 200,
                Message = "succeeded",
                Token = string.Empty
            };
        }
    }
}
