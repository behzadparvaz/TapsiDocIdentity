using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidation: AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidation()
        {
            RuleFor(a=>a.PhoneNumber).NotNull().NotEmpty().Length(11).WithMessage("Enter phone number carefuly");
        }
    }
}
