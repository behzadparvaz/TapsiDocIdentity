using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginByOtp
{
    public class LoginByOtpQuery:IRequest<RegisterSummery>
    {
        public string PhoneNumber { get; set; }
    }
}
