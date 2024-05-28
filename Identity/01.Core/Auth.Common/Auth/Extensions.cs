using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Common.Auth
{
    public static class Extensions
    {
        public static void AddJwt(this IServiceCollection Services, IConfiguration Configuration)
        {
            var options = new JwtOptions();
            var section = Configuration.GetSection("jwt");
            section.Bind(options);
            Services.Configure<JwtOptions>(Configuration.GetSection("jwt"));
            Services.AddSingleton<IJwtHandler, JwtHandler>();
            Services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidIssuer = options.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey))
                    };
                });
        }
    }
}
