using authentication_service.Tests.Helpers;
using authentication_service.Validators;
using Castle.Core.Logging;
using Duende.IdentityServer.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;

namespace authentication_service.Tests.Validators;

public class IdentityResourceOwnerPasswordValidatorTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly Mock<IHttpClientFactory> _factoryMock = new();
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<IdentityResourceOwnerPasswordValidator>> _loggerMock = new();

    public IdentityResourceOwnerPasswordValidatorTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("http://user-service:8000")
        };

        _factoryMock
        .Setup(f => f.CreateClient("UserService"))
        .Returns(_httpClient);
    }

    [Fact]
    public async Task ValidateAsync_ValidCredentials_SetsSuccessResult()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object, _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeFalse();
    }
    [Fact]
    public async Task ValidateAsync_ValidCredentials_SetsSubjectToUserId()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object,
            _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.Subject.GetSubjectId().Should().Be("user-123");
    }
    [Fact]
    public async Task ValidateAsync_ValidCredentials_SetsEmailClaim()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object,
            _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.Subject.Claims
            .Should().Contain(c => c.Type == "email" && c.Value == "john@example.com");
    }
    

    [Fact]
    public async Task ValidateAsync_ValidCredentials_SetsGivenNameClaim()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object, _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.Subject.Claims
            .Should().Contain(c => c.Type == "given_name" && c.Value == "John");
    }
    [Fact]
    public async Task ValidateAsync_ValidCredentials_SetsFamilyNameClaim()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object, _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.Subject.Claims
            .Should().Contain(c => c.Type == "family_name" && c.Value == "Doe");
    }
    [Fact]
    public async Task ValidateAsync_InvalidCredentials_SetsInvalidGrantError()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.Unauthorized,_handlerMock);
        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object, _loggerMock.Object);
        var context = ContextHelper.BuildContext(password: "wrongpassword");

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeTrue();
        context.Result.Error.Should().Be("invalid_grant");
    }

    [Fact]
    public async Task ValidateAsync_FastApiDown_SetsInvalidGrantError()
    {
        // Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var validator = new IdentityResourceOwnerPasswordValidator(_factoryMock.Object, _loggerMock.Object);
        var context = ContextHelper.BuildContext();

        // Act
        await validator.ValidateAsync(context);

        // Assert
        context.Result.IsError.Should().BeTrue();
        context.Result.Error.Should().Be("invalid_grant");
    }
}