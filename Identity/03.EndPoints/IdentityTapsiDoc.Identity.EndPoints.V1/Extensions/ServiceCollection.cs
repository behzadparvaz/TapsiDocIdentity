using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;

internal static class ApplicationDependencyRegistrator
{

    internal static IServiceCollection AddCerberusIdentity(
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
                    ValidIssuer = "http://cerberus.membership",
                    ValidAudiences = ApplicationTokens.Tokens.Keys
                };
            });
        return services;
    }

    internal static IServiceCollection AddAspNetIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
        {
            options.Stores.MaxLengthForKeys = 36;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<DataBaseContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
