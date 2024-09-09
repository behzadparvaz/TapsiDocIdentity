using IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services.TapsiSso;

namespace IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services
{
    public interface ITapsiSsoService
    {
        Task<GetTokenOutput> GetToken(string code);
    }
}
