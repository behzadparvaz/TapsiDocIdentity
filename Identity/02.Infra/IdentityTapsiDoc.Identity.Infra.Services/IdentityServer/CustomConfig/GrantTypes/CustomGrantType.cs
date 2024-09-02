namespace IdentityTapsiDoc.Identity.EndPoints.V1.IdentityServer.CustomConfig.GrantTypes;

public static class CustomGrantType
{
    public const string SecurityStamp = "SecurityStamp";
}
public static class CustomGrantTypes
{
    public static ICollection<string> SecurityStamp_Credentials =>
        [CustomGrantType.SecurityStamp];
}