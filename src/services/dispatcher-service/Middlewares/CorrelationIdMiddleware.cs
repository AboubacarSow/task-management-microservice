

namespace dispatcher_service.Middlewares;

public class CorrelationIdMiddleware
{
    public CorrelationIdMiddleware(RequestDelegate next)
    {
    }

    public async Task InvokeAsync(HttpContext context)
    {
        throw new NotImplementedException();
    }
}