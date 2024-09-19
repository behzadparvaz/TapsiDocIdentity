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
        private readonly IUserQueryRepository query;

        public RegisterUserCommandHandler(UserManager<User>  userManager , IUserCommandRepository command , IUserQueryRepository query)
        {
            this._userManager = userManager;
            this.command = command;
            this.query = query;
        }

        public async Task<RegisterSummery> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            User user = new()
            {
                UserName = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                FirstName = string.Empty,
                LastName = string.Empty
            };
            
            var result = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (result == null)
            {
                var check = await this.query.CheckSendSMS(request.PhoneNumber);
                if (check)
                    throw new ArgumentException("کد ارسال شد لطفا 3 دقیقه منتظر باشید");
                await this.command.SendOtpCode(request.PhoneNumber);
                return new RegisterSummery
                {
                    HasPassword = false,
                    IsActive = true,
                    IsRegister = false,
                    PhoneNumber = request.PhoneNumber,
                    StatusCode = 200,
                    Message = "succeeded",
                    Token = string.Empty
                };
            }
            else
            {
                if(result.PasswordHash != null)
                    return new RegisterSummery
                    {
                        HasPassword = true,
                        IsActive = true,
                        IsRegister = true,
                        PhoneNumber = request.PhoneNumber,
                        StatusCode = 200,
                        Message = "succeeded",
                        Token = string.Empty
                    };
                var check = await this.query.CheckSendSMS(request.PhoneNumber);
                if (check)
                    throw new ArgumentException("کد ارسال شد لطفا 3 دقیقه منتظر باشید");
                await this.command.SendOtpCode(request.PhoneNumber);
                return new RegisterSummery
                {
                    HasPassword = false,
                    IsActive = true,
                    IsRegister = true,
                    PhoneNumber = request.PhoneNumber,
                    StatusCode = 200,
                    Message = "succeeded",
                    Token = string.Empty
                };
            }
        }
    }
}
