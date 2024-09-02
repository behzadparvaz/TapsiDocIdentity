using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;

namespace IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services
{
    public interface IIdentityService
    {
        Task<string> GenerateTokenAsync(string phoneNumber, int expireInMinute = 60);
        Task<User> LoginByOTPAsync(string phoneNumber, string OTP);
        Task<User> LoginByPasswordAsync(string phoneNumber, string password);
        Task<User> RegisterAsync(string phoneNumber);
        Task SetRolesAsync(User user, params Roles[] roles);
    }
}
