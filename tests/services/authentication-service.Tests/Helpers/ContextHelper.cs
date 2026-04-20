using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace authentication_service.Tests.Helpers;

public class ContextHelper
{
    public static ResourceOwnerPasswordValidationContext BuildContext(
        string email    = "john@example.com",
        string password = "secret123")
    {
        return new ResourceOwnerPasswordValidationContext
        {
            UserName = email,
            Password = password,
            Request = new ValidatedTokenRequest
            {
                Client = new Client
                {
                    ClientId = "test-client"
                }
            }
        };
    }


    public static IsActiveContext BuildIsActiveContext(string subjectId = "user-123")
    {
        var claims = new[] { new System.Security.Claims.Claim("sub", subjectId) };
        var identity = new System.Security.Claims.ClaimsIdentity(claims);
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        return new IsActiveContext(principal, new Client(), "test");
    }


    public static ProfileDataRequestContext BuildProfileContext(string subjectId = "user-123")
    {
        var claims = new[] { new System.Security.Claims.Claim("sub", subjectId) };
        var identity = new System.Security.Claims.ClaimsIdentity(claims);
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        return new ProfileDataRequestContext
        {
            Subject = principal
        };
    }
}