using IdentityTapsiDoc.Identity.Core.Domain.Users.Configurations;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;

internal static class ApplicationDependencyRegistrator
{

    internal static IServiceCollection AddOIDCIdentity(
        this IServiceCollection services, IConfiguration configuration)
    {

        services
            .AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;               
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKeys = ApplicationTokens.Tokens.Values,
                    ValidIssuer = configuration["ValidIssuer"],
                    ValidAudiences = ApplicationTokens.Tokens.Keys
                };
            });
        return services;
    }

    internal static IServiceCollection AddAspNetIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
        .AddEntityFrameworkStores<DataBaseContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection AddTheIdentityServer(
    this IServiceCollection services, IConfiguration configuration)
    {
        IIdentityServerBuilder identityBuilder = services.AddIdentityServer(options =>
        {
            options.Discovery.ShowIdentityScopes = false;
            options.Discovery.ShowApiScopes = false;
            options.Discovery.ShowClaims = false;
            options.Discovery.ShowExtensionGrantTypes = true;
            options.Endpoints.EnableJwtRequestUri = true;
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.IssuerUri = configuration["ValidIssuer"];

            
        })
        .AddInMemoryApiScopes(IdentityConfiguration.GetApiScopes())
        .AddDeveloperSigningCredential()
        .AddInMemoryIdentityResources(IdentityConfiguration.GetIdentityResources())
        .AddInMemoryClients(IdentityConfiguration.GetClients())
        .AddInMemoryApiResources(IdentityConfiguration.GetApiResources())
        .AddAspNetIdentity<User>()
        .AddTestUsers(IdentityConfiguration.GetTestUsers())
        .AddJwtBearerClientAuthentication();

        return services;
    }
}
