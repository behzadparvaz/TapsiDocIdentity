namespace IdentityTapsiDoc.Identity.Infra.Services.IdentityServer.CustomConfig.GrantTypes;

public static class CustomGrantType
{
    public const string SecurityStamp = "SecurityStamp";
}

public static class CustomGrantTypes
{
    public static ICollection<string> SecurityStampCredentials =>
        [CustomGrantType.SecurityStamp];
}