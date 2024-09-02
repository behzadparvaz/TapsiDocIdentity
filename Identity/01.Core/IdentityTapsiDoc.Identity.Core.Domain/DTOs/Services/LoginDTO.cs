namespace IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services;

public class LoginOutput
{
    public required string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
    public int RefreshTokenExpiresIn { get; set; }
    public string? IdToken { get; set; }
}
