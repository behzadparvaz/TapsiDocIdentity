using IdentityTapsiDoc.Identity.Core.Domain.Enums;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService;

public interface IIdentityServiceFactory
{
    IIdentityServiceStrategy GetIdentityServiceStrategy(AuthenticationType authenticationType);
}