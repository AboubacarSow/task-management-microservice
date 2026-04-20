



using System.Net;
using System.Text.Json;

namespace dispatcher_service.Middlewares;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";
   

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UNHANDLED_EXCEPTION {Path} {Method} {CorrelationId}",
                context.Request.Path,
                context.Request.Method,
                context.Request.Headers[CorrelationIdHeader].ToString());

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].ToString();

        var (statusCode, message) = ex switch
        {
            HttpRequestException => (
                HttpStatusCode.BadGateway,
                "Service is unavailable or returned an invalid response"),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred")
        };

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error         = message,
            correlationId = correlationId
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}