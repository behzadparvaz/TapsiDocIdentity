using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration
{
    public sealed class JwtTokenBuilder
    {
        private readonly Dictionary<string, string> claims = new(StringComparer.Ordinal);
        private string audience = "";
        private int expiryInMinutes = 30;
        private string issuer = "";
        private SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("g30zQaUBLjfI0UeDJ1uWVLMLmQSQugUisP1EiPgl8H5i3wscbWyGS84YNbVCP00SHx6ESSBWre6azGvBMQkpdIxuQfv2li7XkGzq1kS5DUOpyzjoKjEZB253K443cY3TI6yzyzpzWMVyTEHyy1BuwccNYOyYfMzBZ7gAAQhIJHwqi1jY3eN1mxLoAXfjAxtSHevCGXHhTaSvuPkXXikVrcJZFeWVUQJO0E2Vlv2qqt5naGqHVvJyVc5Ew56LsidUDFlT8aQUy0wZkIQ1sOpjKRbketkLqwl9B73ZZcvXc1NbH2afmmkFoHR47XuwFhjf"));
        private string subject = "";

        public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            this.securityKey = securityKey;
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            this.subject = subject;
            return this;
        }

        public JwtTokenBuilder AddIssuer(string issuer)
        {
            this.issuer = issuer;
            return this;
        }

        public JwtTokenBuilder AddAudience(string audience)
        {
            this.audience = audience;
            return this;
        }

        public JwtTokenBuilder AddClaim(string type, string value)
        {
            claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddClaims(Dictionary<string, string> claims)
        {
            this.claims.Union(claims);
            return this;
        }

        public JwtTokenBuilder AddExpiry(int expiryInMinutes)
        {
            this.expiryInMinutes = expiryInMinutes;
            return this;
        }

        public JwtToken Build()
        {
            var token = new JwtSecurityToken(
                issuer,
                audience,
                new List<Claim>
                    {
                    new(JwtRegisteredClaimNames.Sub, subject),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }
                    .Union(claims.Select(item => new Claim(item.Key, item.Value))),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(expiryInMinutes),
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256));

            return new JwtToken(token);
        }

    }
}
