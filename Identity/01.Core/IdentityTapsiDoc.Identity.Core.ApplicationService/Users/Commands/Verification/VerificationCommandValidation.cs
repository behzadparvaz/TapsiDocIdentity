using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification
{
    public class VerificationCommandValidation:AbstractValidator<VerificationCommand>
    {
        public VerificationCommandValidation()
        {
            RuleFor(a => a.PhoneNumber).NotNull().NotEmpty().Length(11).WithMessage("Enter phone number carefuly");
            RuleFor(a => a.Code).NotNull().NotEmpty().MinimumLength(4).MaximumLength(7).WithMessage("Enter phone number carefuly");
        }
    }
}
