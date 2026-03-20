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
            Password = password
        };
    }
}