using Microsoft.AspNetCore.Http;
using shared.Utilities;

namespace project_service.Tests.Fixtures;

public class FakeUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var id = user?.FindFirst("sub")?.Value;

        return Guid.Parse(id!);
    }
}
