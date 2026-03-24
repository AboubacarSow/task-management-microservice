using System.Security.Claims;
using authentication_service.Dtos;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace authentication_service.Validators;

public class IdentityResourceOwnerPasswordValidator(HttpClient http, ILogger<IdentityResourceOwnerPasswordValidator> logger)
: IResourceOwnerPasswordValidator
{
    private readonly HttpClient _http = http;
    private readonly ILogger<IdentityResourceOwnerPasswordValidator> _logger = logger;
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var username = context.UserName;
        var clientId = context.Request.ClientId;

        _logger.LogInformation(
            "AUTH_LOGIN_ATTEMPT {Username} {ClientId} {GrantType}",
            username,
            clientId,
            "password"
        );

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
                _logger.LogWarning(
                    "AUTH_LOGIN_FAILED {Username} {ClientId} {StatusCode}",
                    username,
                    clientId,
                    response.StatusCode
                );

                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "Invalid credentials");

                return;
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            if (user is null)
            {
                _logger.LogWarning(
                    "AUTH_LOGIN_FAILED {Username} {ClientId} NULL_USER",
                    username,
                    clientId
                );

                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "Invalid credentials");

                return;
            }

            _logger.LogInformation(
                "AUTH_LOGIN_SUCCESS {UserId} {ClientId}",
                user.Id,
                clientId
            );

            context.Result = new GrantValidationResult(
                subject: user.Id,
                authenticationMethod: "password",
                claims:
                [
                    new Claim("email", user.Email ?? string.Empty),
                    new Claim("preferred_username", user.Username ?? string.Empty),
                    new Claim("given_name", user.FirstName ?? string.Empty),
                    new Claim("family_name", user.LastName ?? string.Empty)
                ]);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "AUTH_SERVICE_UNAVAILABLE {Username} {ClientId}",
                username,
                clientId
            );

            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Authentication service unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AUTH_LOGIN_ERROR {Username} {ClientId}",
                username,
                clientId
            );

            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Authentication failed");
        }
    }
}


