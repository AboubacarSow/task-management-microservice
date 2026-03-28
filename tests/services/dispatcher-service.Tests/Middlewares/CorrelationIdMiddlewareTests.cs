using dispatcher_service.Tests.Helpers;

namespace dispatcher_service.Tests.Middlewares;

public class CorrelationIdMiddlewareTests
{

    private const string CorrelationIdHeader = "X-Correlation-ID";

    [Fact]
    public async Task InvokeAsync_NoCorrelationId_GeneratesAndAddsToResponse()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers.ContainsKey(CorrelationIdHeader)
            .Should().BeTrue();

        context.Response.Headers[CorrelationIdHeader]
            .ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_ExistingCorrelationId_ForwardsSameId()
    {
        // Arrange
        var existingId = Guid.NewGuid().ToString();
        var context    = ContextHelper.BuildHttpContext(correlationId: existingId);
        var middleware = MiddelewareHelper.BuildMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers[CorrelationIdHeader]
            .ToString().Should().Be(existingId);
    }

    [Fact]
    public async Task InvokeAsync_NoCorrelationId_GeneratedIdIsValidGuid()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var correlationId = context.Response.Headers[CorrelationIdHeader].ToString();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Always_CallsNextMiddleware()
    {
        // Arrange
        var nextCalled = false;
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildMiddleware(next: _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_NoCorrelationId_AddsCorrelationIdToRequestHeaders()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.ContainsKey(CorrelationIdHeader)
            .Should().BeTrue();
    }
}