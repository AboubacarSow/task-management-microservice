namespace authentication_service.Tests.Helpers;

public class PayloadHelper
{
    public static object ValidUserPayload() => new
    {
        id         = "user-123",
        email      = "john@example.com",
        first_name = "John",
        last_name  = "Doe"
    };
}
