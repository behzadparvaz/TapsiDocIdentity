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
        
        services.Configure<IdentityOptions>(option =>
        {
            option.Password = new PasswordOptions()
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8,
                RequiredUniqueChars = 1
            };

            //Lokout Setting
            option.Lockout.MaxFailedAccessAttempts = 3;
            option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(3);

            //SignIn Setting
            option.SignIn.RequireConfirmedAccount = false;
            option.SignIn.RequireConfirmedEmail = false;
            option.SignIn.RequireConfirmedPhoneNumber = false;

        });

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
                ValidIssuer = configurationManager["ValidIssuer"],
                ValidAudiences = ApplicationTokens.Tokens.Keys
            };
        });

        services.AddIdentity<User, Role>()
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
