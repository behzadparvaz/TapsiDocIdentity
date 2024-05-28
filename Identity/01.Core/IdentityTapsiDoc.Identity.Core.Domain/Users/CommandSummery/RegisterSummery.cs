using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery
{
    public class RegisterSummery
    {
        public string PhoneNumber { get; set; }
        public bool IsRegister { get; set; }
        public bool HasPassword { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
