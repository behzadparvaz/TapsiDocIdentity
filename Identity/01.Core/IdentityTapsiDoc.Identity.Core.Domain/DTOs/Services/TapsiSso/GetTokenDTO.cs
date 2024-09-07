using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;

namespace IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services.TapsiSso
{
    public class GetTokenOutput
    {
        public required string Token { get; set; }
        public required User User { get; set; }
    }
}
