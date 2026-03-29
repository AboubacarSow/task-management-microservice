using dispatcher_service.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace dispatcher_service.Tests.Helpers;

public class MiddelewareHelper
{
    public static CorrelationIdMiddleware BuildCorrelationMiddleware(RequestDelegate? next = null)
    {
        next ??= _ => Task.CompletedTask;
        return new CorrelationIdMiddleware(next);
    }

    public static GlobalExceptionHandler BuildGlobalExceptionHandler(RequestDelegate next)
    {
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        return new GlobalExceptionHandler(next, logger.Object);
    }
}