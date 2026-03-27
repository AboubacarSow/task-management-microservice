using System.Security.Claims;
using authentication_service.Dtos;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Serilog.Context;

namespace authentication_service.Services;

public class UserProfileService(HttpClient http, ILogger<UserProfileService> logger,
    IHttpContextAccessor httpContextAccessor) :IProfileService
{
    private readonly HttpClient _http = http;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<UserProfileService> _logger=logger;

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var correlationId = GetCorrelationId();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://user-service:5002/api/users/profile/{sub}");
                request.Headers.Add("X-Correlation-ID", correlationId);

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "USER_PROFILE_FETCH_FAILED {UserId} {StatusCode} {Service} {CorrelationId}",
                        sub,
                        response.StatusCode,
                        "IdentityService",
                        correlationId
                    );
                    return;
                }

                var user = await response.Content.ReadFromJsonAsync<UserDto>();

                if (user is null)
                {
                        _logger.LogWarning("USER_PROFILE_NULL_RESPONSE {UserId} {Service} {CorrelationId}",
                            sub,
                            "IdentityService",
                            correlationId
                        );
                    return;
                }

                var claims = new List<Claim>
                {
                    new Claim("given_name", user.FirstName),
                    new Claim("family_name", user.LastName),
                    new Claim("email", user.Email)
                };

                claims = claims
                    .Where(claim => context.RequestedClaimTypes.Contains(claim.Type))
                    .ToList();

                context.IssuedClaims.AddRange(claims);
            }
            
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "USER_SERVICE_UNAVAILABLE {UserId} {Service} {CorrelationId}",
                    sub,
                    "IdentityService",
                    correlationId
                );
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "USER_PROFILE_DESERIALIZATION_FAILED {UserId} {Service} {CorrelationId}",
                    sub,
                    "IdentityService",
                    correlationId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "UNEXPECTED_ERROR_IN_GETPROFILE {UserId} {Service} {CorrelationId}",
                    sub,
                    "IdentityService",
                    correlationId
                );
            }
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var correlationId = GetCorrelationId();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://user-service:5002/api/users/{sub}/active");
                request.Headers.Add("X-Correlation-ID", correlationId);

                var response = await _http.SendAsync(request);
                context.IsActive = response.IsSuccessStatusCode;

                if (!context.IsActive)
                {
                    _logger.LogWarning(
                        "USER_ISACTIVE_FAILED {UserId} {StatusCode} {Service}",
                        sub,
                        response.StatusCode,
                        "IdentityService"
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                context.IsActive = false;
                _logger.LogError(
                    ex,
                    "USER_SERVICE_UNAVAILABLE_ISACTIVE {UserId} {Service}",
                    sub,
                    "IdentityService"
                );
            }
            catch (Exception ex)
            {
                context.IsActive = false;
                _logger.LogError(
                    ex,
                    "UNEXPECTED_ERROR_IN_ISACTIVE {UserId} {Service}",
                    sub,
                    "IdentityService"
                );
            }
        }
    }

    private string GetCorrelationId()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault()
               ?? Guid.NewGuid().ToString(); 
    }
}