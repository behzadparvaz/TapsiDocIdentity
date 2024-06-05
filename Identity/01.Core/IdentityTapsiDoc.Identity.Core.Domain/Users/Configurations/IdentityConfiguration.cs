using IdentityModel;
using IdentityServer4.Models;
using static IdentityModel.OidcConstants;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Configurations;

public sealed class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone()
                };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
                    {
                        new ApiScope("ProductService", 
                            "Product Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            })
                    };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
                    {
                        new ApiResource("ProductService", "Product Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "ProductService"
                            })
                    };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
                 new Client
                 {
                     ClientId="5aa3ac", // web application
                     ClientSecrets=new List<Secret>
                     {
                         new Secret("97a87c".Sha256())
                     }

                 , AllowedGrantTypes=IdentityServer4.Models.GrantTypes.Implicit,
                   RedirectUris={"https://localhost:7294/signin-oidc" },
                   PostLogoutRedirectUris={"https://localhost:7294/signout-callback-oidc"},

                     AllowedScopes=new List<string>
                     {
                         IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                              IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                              IdentityServer4.IdentityServerConstants.StandardScopes.Email,
                              IdentityServer4.IdentityServerConstants.StandardScopes.Phone
                     },
                     RequireConsent=false
                 },
                 // product service
                 new Client
                 {
                     ClientId="5bd46062730c4055aa3acfbefd7f962c",
                     ClientSecrets=new List<Secret>{ new Secret("97e0769e4c334cb8a80f01dec2daa87c".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ClientCredentials,
                     AllowedScopes =
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                StandardScopes.OfflineAccess,
                                "ProductService"
                            },
                     ClientName = "ProductManagement",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding,
                 }
            };
    }
}

