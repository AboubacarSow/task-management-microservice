<<<<<<< HEAD
namespace project_service.Data.Utilities;

public class DatabaseSettings
{
    public string ConnectionStrings { get; set; }=default!;
    public string Database { get; set; } = default!;
    public string ProjectCollection { get; set; }= default!;
}
public interface IUserContext
{
    Guid GetUserId();
}
public class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var id = user?.FindFirst("sub")?.Value;

        return Guid.Parse(id!);
    }
}
=======
namespace project_service.Data.Utilities;

public class DatabaseSettings
{
    public string ConnectionStrings { get; set; }=default!;
    public string Database { get; set; } = default!;
    public string ProjectCollection { get; set; }= default!;
}
>>>>>>> 05b451b (new_update)
