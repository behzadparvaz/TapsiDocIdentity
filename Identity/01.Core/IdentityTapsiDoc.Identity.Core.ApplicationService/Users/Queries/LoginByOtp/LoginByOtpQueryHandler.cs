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
        private readonly IUserQueryRepository query;

        public LoginByOtpQueryHandler(UserManager<User> userManager , IUserCommandRepository command , IUserQueryRepository query)
        {
            this._userManager = userManager;
            this.command = command;
            this.query = query;
        }

        public async Task<RegisterSummery> Handle(LoginByOtpQuery request, CancellationToken cancellationToken)
        {
            var result = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (result != null)
            {
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
            else
                throw new ArgumentException("شماره ارسالی معتبر نیست");
        }
    }
}
