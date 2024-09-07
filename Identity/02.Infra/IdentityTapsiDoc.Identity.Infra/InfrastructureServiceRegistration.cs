using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Infra.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTapsiDoc.Identity.Infra;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddScoped<ITapsiSsoService, TapsiSsoService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
