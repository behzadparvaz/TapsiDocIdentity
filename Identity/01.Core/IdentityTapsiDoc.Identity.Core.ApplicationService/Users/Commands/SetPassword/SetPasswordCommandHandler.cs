using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword
{
    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public SetPasswordCommandHandler(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<bool> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.Password.Equals(request.ConfrimPassword))
            {
                var result = await this._userManager.FindByNameAsync(request.PhoneNumber);
                if(result == null)
                    throw new ArgumentException("");
                if (result.PasswordHash == null)
                   await _userManager.AddPasswordAsync(result, request.Password);
                return true;
            }
            else
                throw new ArgumentException("password and confrim pawwsord not mached");
        }
    }
}
