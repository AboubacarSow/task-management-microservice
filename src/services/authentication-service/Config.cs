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
            new ApiScope("project_fullpermission", "Full permission for project operations"),
            new ApiScope("agent_fullpermission", "Agent Service Access"),
            new ApiScope("user_fullpermission", "User Service Access")
        ];
    
    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("project-service"){Scopes={"project_fullpermission"}},
        new ApiResource("agent-service"){Scopes={"agent_fullpermission"}},
        new ApiResource("user-service"){Scopes={"user_fullpermission"}},
        new ApiResource("profile")
        {
            UserClaims =
            {
                "email",
                "given_name",
                "family_name"
            }
        }

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
                    "email",
                    "project_fullpermission",
                    "agent_fullpermission",
                    "user_fullpermission",
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
