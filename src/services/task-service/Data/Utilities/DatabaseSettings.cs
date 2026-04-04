namespace task_service.Data.Utilities;

public class DatabaseSettings
{
    public string ConnectionStrings { get; set; } = default!;
    public string Database { get; set; } = default!;
    public string TaskCollection { get; set; }=default!;
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