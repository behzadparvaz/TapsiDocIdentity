using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories
{
    public interface IUserQueryRepository
    {
        Task<bool> CheckCode(string phoneNumber, string code);
        Task<bool> CheckSendSMS(string phoneNumber);
    }
}
