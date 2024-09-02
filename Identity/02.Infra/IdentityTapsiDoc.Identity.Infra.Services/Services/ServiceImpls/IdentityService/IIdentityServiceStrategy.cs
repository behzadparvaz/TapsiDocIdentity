using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService;

public interface IIdentityServiceStrategy
{
    AuthenticationType AuthenticationType { get; }
    Task<GetTokenOutput> GetToken(string userName);
    Task<GetUserInfoOutput> GetUserInfo(string accessToken);
}