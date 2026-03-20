using System.Security.Claims;
using authentication_service.Dtos;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;

namespace authentication_service.Services;

public class UserProfileService(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task GetProfileAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        try
        {
            var response = await _http.GetAsync($"/api/users/{sub}");

            if (!response.IsSuccessStatusCode)
                return;

            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            if (user is null)
                return;

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
            Console.WriteLine($"UserService unreachable for user {sub}: {ex.Message}");
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