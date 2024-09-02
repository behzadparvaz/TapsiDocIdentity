﻿using System.Text.Json.Serialization;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Models;

public class GetTokenOutput
{
    public required string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
    public int RefreshTokenExpiresIn { get; set; }
    public string? IdToken { get; set; }
}