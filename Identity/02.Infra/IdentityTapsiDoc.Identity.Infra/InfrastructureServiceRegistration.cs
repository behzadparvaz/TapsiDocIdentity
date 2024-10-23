using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace IdentityTapsiDoc.Identity.Infra;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddScoped<ITapsiSsoService, TapsiSsoService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSecurityService(configurationManager);
        return services;
    }
    public static IServiceCollection AddSecurityService(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var connectionString =
            configurationManager.GetConnectionString("IdentityServerConnection") ??
            throw new NullReferenceException("CONNECTION STRING NOT FOUND");

        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
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
                ValidIssuer = "http://localhost:35200",
                ValidAudiences = ApplicationTokens.Tokens.Keys
            };
        });

        services.AddIdentity<User, Role>(options =>
        {
            options.Stores.MaxLengthForKeys = 36;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            //Password Setting
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

        services.AddIdentityServer()
            .AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectionString, sql =>
                    {
                        sql.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    });
                };
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectionString, sql =>
                    {
                        sql.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    });
                };
            })
            .AddDeveloperSigningCredential();


        return services;
    }
}
