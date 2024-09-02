using IdentityTapsiDoc.Identity.Core.Domain.Enums;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService;

public class IdentityServiceFactory(IEnumerable<IIdentityServiceStrategy> availableStrategies) : IIdentityServiceFactory
{
    public IIdentityServiceStrategy GetIdentityServiceStrategy(AuthenticationType authenticationType)
    {
        var supportedStrategy = availableStrategies
            .FirstOrDefault(z => z.AuthenticationType == authenticationType);

        if (supportedStrategy == null)
            throw new NotImplementedException
                ($"IDENTITY SERVICE STRATEGY {authenticationType}NOT IMPLEMENTED");

        return supportedStrategy;
    }
}