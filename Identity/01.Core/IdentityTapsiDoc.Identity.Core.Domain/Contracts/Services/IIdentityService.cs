using IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;

namespace IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;

public interface IIdentityService
{
    Task<LoginOutput> Login(string userName, AuthenticationType authType);
    Task<string> GetUserInfo(string accessToken);

}