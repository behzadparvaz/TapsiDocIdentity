using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification;
using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using IdentityTapsiDoc.Identity.EndPoints.V1.Extensions;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext;
using IdentityTapsiDoc.Identity.Infra.Data.Query.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Redis;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataBaseContext>(
    p => p.UseSqlServer("Server=10.192.30.35;Database=Identitydb;MultipleActiveResultSets=true;User ID=sa;Password=Aliz@123;TrustServerCertificate=True; connect timeout=3000"));

builder.Services.AddAspNetIdentity()
    .AddCerberusIdentity(builder.Configuration);

//builder.Services.AddIdentity<User, Role>()
//.AddEntityFrameworkStores<DataBaseContext>()
//    .AddDefaultTokenProviders()
//    .AddRoles<Role>();

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer(options =>
//    {
//        options.SaveToken = true;
//        options.RequireHttpsMetadata = false;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ClockSkew = TimeSpan.Zero,
//            IssuerSigningKeys = ApplicationTokens.Tokens.Values,
//            ValidIssuer = "",
//            ValidAudiences = ApplicationTokens.Tokens.Keys
//        };
//    });

//builder.Services.Configure<IdentityOptions>(option => {
//    option.Lockout.MaxFailedAccessAttempts = 5;
//    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//});


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
    .AddTransient<IRequestHandler<SetPasswordCommand, bool>, SetPasswordCommandHandler>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
