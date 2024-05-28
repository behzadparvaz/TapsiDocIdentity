using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword
{
    public class SetPasswordCommand:IRequest<bool>
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfrimPassword { get; set; }
    }
}
