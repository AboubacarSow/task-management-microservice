using Duende.IdentityServer.Models;

namespace authentication_service;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("project_service", "Project Service Access"),
            new ApiScope("agent_service", "Agent Service Access"),
            new ApiScope("user_service", "User Service Access")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId   = "postman-client",
                ClientName = "Postman Testing Client",

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets =
                {
                    new Secret("postman-secret".Sha256())
                },

                AllowedScopes =
                {
                    "openid",
                    "profile",
                    "project_service",
                    "agent_service",
                    "user_service",
                },

                AccessTokenLifetime          = 3600,   
                AbsoluteRefreshTokenLifetime = 604800, 

                AllowOfflineAccess     = true,
                RefreshTokenUsage      = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,

                AlwaysIncludeUserClaimsInIdToken = true
            }
        ];
}
