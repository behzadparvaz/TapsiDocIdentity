using IdentityServer4;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.Profiles;
using IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;

internal static class ApplicationDependencyRegistrator
{
    internal static IServiceCollection AddSecurityService(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.Stores.MaxLengthForKeys = 36;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                //Lokout Setting
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(3);

                //SignIn Setting
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<DataBaseContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryApiScopes(IdentityConfiguration.GetApiScopes())
            .AddInMemoryIdentityResources(IdentityConfiguration.GetIdentityResources())
            .AddInMemoryClients(IdentityConfiguration.GetClients())
            .AddInMemoryApiResources(IdentityConfiguration.GetApiResources())
            .AddAspNetIdentity<User>()
            .AddTestUsers(IdentityConfiguration.GetTestUsers())
            .AddExtensionGrantValidator<CustomSecurityStampValidator>()
            .AddProfileService<CustomProfileService>()
            .AddDeveloperSigningCredential();
        return services;
        
        return services;
    }
}