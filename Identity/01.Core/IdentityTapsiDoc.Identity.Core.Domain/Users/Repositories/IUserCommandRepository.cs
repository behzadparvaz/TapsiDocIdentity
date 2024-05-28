using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories
{
    public interface IUserCommandRepository
    {
        Task Create(User user);
        Task<bool> SendOtpCode(string phoneNumber);
    }
}
