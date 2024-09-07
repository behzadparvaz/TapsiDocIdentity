using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services
{
    public interface IIdentityService
    {
        Task<string> GenerateTokenAsync(string phoneNumber, int expireInMinute = 60, string issuer = "");
        Task<User> RegisterAsync(string phoneNumber);
        Task SetRolesAsync(User user, params Roles[] roles);
    }
}
