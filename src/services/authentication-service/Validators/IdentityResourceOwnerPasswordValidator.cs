using System.Security.Claims;
using authentication_service.Dtos;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace authentication_service.Validators;

public class IdentityResourceOwnerPasswordValidator(HttpClient http) : IResourceOwnerPasswordValidator
{
    private readonly HttpClient _http = http;

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var payload = new
            {
                username = context.UserName,
                password = context.Password
            };

            var response = await _http.PostAsJsonAsync("/api/auth/validate", payload);

            if (!response.IsSuccessStatusCode)
            {
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "Invalid credentials");
                return;
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            context.Result = new GrantValidationResult(
                subject: user!.Id,
                authenticationMethod: "password",
                claims: new[]
                {
                    new Claim("email",              user.Email),
                    new Claim("preferred_username", user.Username),
                    new Claim("given_name",         user.FirstName),
                    new Claim("family_name",        user.LastName)
                });
        }
        catch (HttpRequestException)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Authentication service unavailable");
        }
    }
}


