using System.Security.Claims;
using authentication_service.Dtos;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;

namespace authentication_service.Services;

public class UserProfileService(HttpClient http, ILogger<UserProfileService> logger)
{
    private readonly HttpClient _http = http;
    private readonly ILogger<UserProfileService> _logger=logger;

    public async Task GetProfileAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        try
        {
            var response = await _http.GetAsync($"/api/users/{sub}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                "UserService returned {StatusCode} for user {UserId}",
                response.StatusCode,
                sub
                );
                return;
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            if (user is null)
            {
                 _logger.LogWarning("UserService returned null for user {UserId}", sub);
                return;
            }

            context.IssuedClaims.AddRange(
            [
                new Claim("email",              user.Email),
                new Claim("preferred_username", user.Username),
                new Claim("given_name",         user.FirstName),
                new Claim("family_name",        user.LastName)
            ]);
        }
        
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "UserService unreachable for user {UserId}", sub);
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize user data for user {UserId}", sub);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetProfileAsync for user {UserId}", sub);
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        try
        {
            var sub = context.Subject.GetSubjectId();
            var response = await _http.GetAsync($"/api/users/{sub}/active");
            context.IsActive = response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            context.IsActive = false;
        }
    }
}