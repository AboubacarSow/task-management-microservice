<<<<<<< HEAD
using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace authentication_service.Tests.Helpers;
public class ResponseHelper
{
    public static  void SetupResponse(HttpStatusCode statusCode,
    Mock<HttpMessageHandler> _handlerMock, object? body = null)
    {
        var json = body is not null
            ? JsonSerializer.Serialize(body)
            : string.Empty;

        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    } 
}
=======
using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace authentication_service.Tests.Helpers;
public class ResponseHelper
{
    public static  void SetupResponse(HttpStatusCode statusCode,
    Mock<HttpMessageHandler> _handlerMock, object? body = null)
    {
        var json = body is not null
            ? JsonSerializer.Serialize(body)
            : string.Empty;

        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    } 
}
>>>>>>> 05b451b (new_update)
