using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using MediatR;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser
{
    public class RegisterUserCommand:IRequest<RegisterSummery>
    {
        public string PhoneNumber { get; set; }
    }
}
