using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace shared.Utilities;


public interface IUserContext
{
    Guid GetUserId();
}
public class UserContextClaimPrincipal(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetUserId()
    {
       var user = _httpContextAccessor.HttpContext?.User;

        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var guid))
            throw new UnauthorizedAccessException("Invalid user id");

        return guid;
    }
}