using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginByOtp;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;
using IdentityTapsiDoc.Identity.Infra;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Data.Persistence.SeedData;
using IdentityTapsiDoc.Identity.Infra.Data.Query.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Redis;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataBaseContext>(
    p => p.UseSqlServer(builder.Configuration.GetSection("Connection:ConnectionString").Value));

builder.Services.Configure<IdentityOptions>(option =>
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

builder.Services
    .AddAspNetIdentity()
    .AddOIDCIdentity(builder.Configuration)
    .AddTheIdentityServer();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton(typeof(IRedisClientsManager),
    new RedisManagerPool(builder.Configuration.GetSection("Redis:RedisConnectionString").Value));

builder.Services.AddSingleton<IUserCommandRepositoryRedis, UserCommandRepositoryRedis>();
builder.Services.AddSingleton<IUserQueryRepositoryRedis, UserQueryRepositoryRedis>();

builder.Services.AddMediatR(cfg =>
     cfg.RegisterServicesFromAssembly(typeof(Ping).Assembly))
    .AddTransient<IUserCommandRepository, UserCommandRepository>()
    .AddTransient<IUserQueryRepository, UserQueryRepository>()
    .AddTransient<IRequestHandler<RegisterUserCommand, RegisterSummery>, RegisterUserCommandHandler>()
    .AddTransient<IRequestHandler<VerificationCommand, RegisterSummery>, VerificationCommandHandler>()
    .AddTransient<IRequestHandler<SetPasswordCommand, bool>, SetPasswordCommandHandler>()
    .AddTransient<IRequestHandler<LoginUserQuery, RegisterSummery>, LoginUserQueryHandler>()
    .AddTransient<IRequestHandler<LoginByOtpQuery, RegisterSummery>, LoginByOtpQueryHandler>()
    .AddTransient<IRequestHandler<LoginWithTapsiSSOQuery, RegisterSummery>, LoginWithTapsiSSOQHandler>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("fixed-register-user", httpContext =>
    {
        var phoneNumber = httpContext.GetPhoneNumberFromBodyAsync().GetAwaiter().GetResult();

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: phoneNumber,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.AddPolicy("fixed-verify-user", httpContext =>
    {
        var phoneNumber = httpContext.GetPhoneNumberFromBodyAsync().GetAwaiter().GetResult();

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: phoneNumber,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins= builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
var policyName = builder.Configuration.GetSection("Cors:Name").Value;

builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName!, policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(allowedOrigins!)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

//app.UseHttpsRedirection();
app.UseIdentityServer();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.UseCors(policyName!);

//(new DatabaseInit()).InitializeDatabase(app);
app.Run();


