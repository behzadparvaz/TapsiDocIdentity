using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginByOtp;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;
using IdentityTapsiDoc.Identity.Infra;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Data.Query.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Redis;
using System.Net.NetworkInformation;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataBaseContext>(
    p => p.UseSqlServer(builder.Configuration.GetSection("Connection:ConnectionString").Value));

builder.Services
    .AddAspNetIdentity()
    .AddOIDCIdentity(builder.Configuration)
    .AddTheIdentityServer();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<IdentityOptions>(option =>
{

    //Password Setting
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
    option.Password.RequiredLength = 3;
    option.Password.RequiredUniqueChars = 1;

    //Lokout Setting
    option.Lockout.MaxFailedAccessAttempts = 3;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(3);

    //SignIn Setting
    option.SignIn.RequireConfirmedAccount = false;
    option.SignIn.RequireConfirmedEmail = false;
    option.SignIn.RequireConfirmedPhoneNumber = false;

});


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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseIdentityServer();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.UseCors(x => x
       .AllowAnyMethod()
       .AllowAnyHeader()
       .SetIsOriginAllowed(origin => true)
       .AllowCredentials());

app.Run();
