using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword
{
    public class SetPasswordCommandValidation:AbstractValidator<SetPasswordCommand>
    {
        public SetPasswordCommandValidation()
        {
            RuleFor(x => x.Password).NotEmpty().NotNull().MinimumLength(3).WithMessage("Password is required");
            RuleFor(x => x.ConfrimPassword).NotEmpty().NotNull().MinimumLength(3).WithMessage("confrim Password is required");
        }

       
    }
}
