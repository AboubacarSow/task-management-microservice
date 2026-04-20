<<<<<<< HEAD
using System.Net;
using System.Text.Json;
using dispatcher_service.Middlewares;
using dispatcher_service.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace dispatcher_service.Tests.Middlewares;

public class GlobalExceptionHandlerMiddlewareTests
{
   
    [Fact]
    public async Task InvokeAsync_NoException_PassesThroughNormally()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ =>
        {
            context.Response.StatusCode = 200;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_Returns500()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_ReturnsValidJson()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var act = () => JsonDocument.Parse(body);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_ResponseContainsErrorMessage()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.TryGetProperty("error", out _).Should().BeTrue();
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_ResponseContainsCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var context       = ContextHelper.BuildHttpContext(correlationId: correlationId);
        var middleware    = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.TryGetProperty("correlationId", out var id).Should().BeTrue();
        id.GetString().Should().Be(correlationId);
    }


    [Fact]
    public async Task InvokeAsync_UnhandledException_SetsContentTypeToJson()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Contain("application/json");
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_LogsError()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        var middleware = new GlobalExceptionHandler(
            _ => throw new Exception("Something went wrong"),
            loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
public async Task InvokeAsync_HttpRequestException_Returns502()
{
    // Arrange
    var context    = ContextHelper.BuildHttpContext();
    var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => 
        throw new HttpRequestException("Service unavailable"));

    // Act
    await middleware.InvokeAsync(context);

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
    var json = JsonDocument.Parse(body);

    json.RootElement.TryGetProperty("error", out var error).Should().BeTrue();
    error.GetString().Should().Contain("unavailable");

    // Assert
    context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);


}


[Fact]
public async Task InvokeAsync_HttpRequestException_ResponseContainsBadGatewayMessage()
{
    // Arrange
    var context    = ContextHelper.BuildHttpContext();
    var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => 
        throw new HttpRequestException("Service unavailable"));

    // Act
    await middleware.InvokeAsync(context);

    // Assert
    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
    var json = JsonDocument.Parse(body);

    json.RootElement.TryGetProperty("error", out var error).Should().BeTrue();
    error.GetString().Should().Contain("unavailable");
}
=======
using System.Net;
using System.Text.Json;
using dispatcher_service.Middlewares;
using dispatcher_service.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace dispatcher_service.Tests.Middlewares;

public class GlobalExceptionHandlerMiddlewareTests
{
   
    [Fact]
    public async Task InvokeAsync_NoException_PassesThroughNormally()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ =>
        {
            context.Response.StatusCode = 200;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_Returns500()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_ReturnsValidJson()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var act = () => JsonDocument.Parse(body);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_ResponseContainsErrorMessage()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.TryGetProperty("error", out _).Should().BeTrue();
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_ResponseContainsCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var context       = ContextHelper.BuildHttpContext(correlationId: correlationId);
        var middleware    = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.TryGetProperty("correlationId", out var id).Should().BeTrue();
        id.GetString().Should().Be(correlationId);
    }


    [Fact]
    public async Task InvokeAsync_UnhandledException_SetsContentTypeToJson()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => throw new Exception("Something went wrong"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Contain("application/json");
    }
    [Fact]
    public async Task InvokeAsync_UnhandledException_LogsError()
    {
        // Arrange
        var context    = ContextHelper.BuildHttpContext();
        var loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        var middleware = new GlobalExceptionHandler(
            _ => throw new Exception("Something went wrong"),
            loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
public async Task InvokeAsync_HttpRequestException_Returns502()
{
    // Arrange
    var context    = ContextHelper.BuildHttpContext();
    var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => 
        throw new HttpRequestException("Service unavailable"));

    // Act
    await middleware.InvokeAsync(context);

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
    var json = JsonDocument.Parse(body);

    json.RootElement.TryGetProperty("error", out var error).Should().BeTrue();
    error.GetString().Should().Contain("unavailable");

    // Assert
    context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);


}


[Fact]
public async Task InvokeAsync_HttpRequestException_ResponseContainsBadGatewayMessage()
{
    // Arrange
    var context    = ContextHelper.BuildHttpContext();
    var middleware = MiddelewareHelper.BuildGlobalExceptionHandler(_ => 
        throw new HttpRequestException("Service unavailable"));

    // Act
    await middleware.InvokeAsync(context);

    // Assert
    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
    var json = JsonDocument.Parse(body);

    json.RootElement.TryGetProperty("error", out var error).Should().BeTrue();
    error.GetString().Should().Contain("unavailable");
}
>>>>>>> 05b451b (new_update)
}