

namespace dispatcher_service.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private readonly string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].ToString();

        if (string.IsNullOrEmpty(correlationId))
            correlationId = Guid.NewGuid().ToString();

        context.Request.Headers[CorrelationIdHeader]  = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        await _next(context);
    }
}