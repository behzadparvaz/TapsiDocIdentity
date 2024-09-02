using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginByOtp;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginUser;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Data.Query.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Redis;
using System.Net.NetworkInformation;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.EndPoints.V1;
using IdentityTapsiDoc.Identity.Infra.Services.Services;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Strategies;
using System.Reflection;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataBaseContext>(
    p => p.UseSqlServer(builder.Configuration.GetSection("Connection:ConnectionString").Value));

builder.Services.AddSecurityService();
//add auto mapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

builder.Services.AddSingleton(typeof(IRedisClientsManager),
    new RedisManagerPool(builder.Configuration.GetSection("Redis:RedisConnectionString").Value));

builder.Services.AddSingleton<IUserCommandRepositoryRedis, UserCommandRepositoryRedis>();
builder.Services.AddSingleton<IUserQueryRepositoryRedis, UserQueryRepositoryRedis>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddScoped<IIdentityServiceStrategy, TapsiDr>();
builder.Services.AddScoped<IIdentityServiceStrategy, TapsiSSO>();
builder.Services.AddScoped<IIdentityServiceFactory, IdentityServiceFactory>();

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

//Add MediatR
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
//});

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