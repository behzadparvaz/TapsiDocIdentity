using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityTapsiDoc.Identity.Core.Domain.Users.LegacyIntegration;
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
                            }),
                        new ApiScope("V00001",
                            "VendorV1 Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            }),
                        new ApiScope("V00002",
                            "VendorV2 Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            }),
                        new ApiScope("V00003",
                            "VendorV3 Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            }),
                         new ApiScope("V00004",
                            "VendorV4 Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            }),
                        new ApiScope("ZapDeliver",
                            "Delivery Service",
                            new List<string>
                            {
                                JwtClaimTypes.Subject,
                                JwtClaimTypes.Role,
                                JwtClaimTypes.Name,
                                JwtClaimTypes.Email
                            })
                    };
    }

    public static List<TestUser> GetTestUsers()
    {
        return new List<TestUser>()
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "Z@p",
            Password = "123456"
        }
    };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
                    {
                        new ApiResource("ProductServiceApi", "Product Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "ProductServiceApi"
                            }),
                        new ApiResource("V00001Api", "Vendor Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "V00001Api"
                            }),
                        new ApiResource("V00002Api", "Vendor Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "V00002Api"
                            }),
                         new ApiResource("V00003Api", "Vendor Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "V00003Api"
                            }),
                         new ApiResource("V00004Api", "Vendor Service",
                            new List<string>
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.OfflineAccess,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                "V00004Api"
                            }),
                        new ApiResource("ZapDeliver")
                            {
                                ApiSecrets = {new Secret("secret".Sha256())}
                            }
                    };
    }

    public static IEnumerable<Client> GetClients()
    {
        var ss = ApplicationTokens.Tokens.Values;
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
                 },
                 // Vendor
                 new Client
                 {
                     ClientId="isar",
                     ClientSecrets=new List<Secret>{ new Secret("Is@@r".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ClientCredentials,
                     AllowedScopes =
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                StandardScopes.OfflineAccess,
                                "V00001"
                            },
                     ClientName = "isar",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding
                 },
                 new Client
                 {
                     ClientId="13@baN",
                     ClientSecrets=new List<Secret>{ new Secret("aB@an".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ClientCredentials,
                     AllowedScopes =
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                StandardScopes.OfflineAccess,
                                "V00002"
                            },
                     ClientName = "13aban",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding
                 },
                   new Client
                 {
                     ClientId="M@tlaBiKhA",
                     ClientSecrets=new List<Secret>{ new Secret("M@tlaBiKhA7373136".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ClientCredentials,
                     AllowedScopes =
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                StandardScopes.OfflineAccess,
                                "V00003"
                            },
                     ClientName = "Matlabikha",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding
                 },
                        new Client
                 {
                     ClientId="MosbatGreen",
                     ClientSecrets=new List<Secret>{ new Secret("mosbatsabz88707989".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ClientCredentials,
                     AllowedScopes =
                            {
                                StandardScopes.OpenId,
                                StandardScopes.Profile,
                                StandardScopes.Phone,
                                StandardScopes.Email,
                                StandardScopes.OfflineAccess,
                                "V00004"
                            },
                     ClientName = "Mosbatsabz",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding
                 },
                 new Client
                 {
                     ClientId="Zap",
                     ClientSecrets=new List<Secret>{ new Secret("Z@pExpre$$".Sha256()) },
                     RequireClientSecret  = true,
                     AllowedGrantTypes=IdentityServer4.Models.GrantTypes.ResourceOwnerPassword,
                     AllowedScopes =
                            {
                                "ZapDeliver"
                            },
                     ClientName = "ZapExpress",
                     AccessTokenLifetime = 12 * 60 * 60, /* 12 hours */
                     IdentityTokenLifetime= 12 * 60 * 60, /* 12 hours */
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Sliding
                 }
            };
    }
}

