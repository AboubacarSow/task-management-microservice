using authentication_service.Services;
using authentication_service.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;

namespace authentication_service.Tests.Services;

public class UserProfileServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly Mock<IHttpClientFactory> _factoryMock = new();
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<UserProfileService>> _loggerMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    public UserProfileServiceTests()
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
    public async Task GetProfileAsync_ValidUser_SetsEmailClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object,_httpContextAccessorMock.Object);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should()
            .Contain(c => c.Type == "email" && c.Value == "john@example.com");
    }

    [Fact]
    public async Task GetProfileAsync_ValidUser_SetsGivenNameClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK, _handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should().Contain(c => c.Type == "given_name" && c.Value == "John");
    }

    [Fact]
    public async Task GetProfileDataAsync_ValidUser_SetsFamilyNameClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should().Contain(c => c.Type == "family_name" && c.Value == "Doe");
    }

    [Fact]
    public async Task GetProfileAsync_UserNotFound_NoClaimsIssued()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.NotFound,_handlerMock);
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildProfileContext();

        // Act
        await service.GetProfileDataAsync(context);

        context.IssuedClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProfileAsync_ApiDown_NoClaimsIssued()
    {
        // Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildProfileContext();

        // Act
        await service.GetProfileDataAsync(context);

        // Assert
        context.IssuedClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task IsActiveAsync_ActiveUser_SetsIsActiveTrue()
    {
        // Arrange
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock);
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildIsActiveContext();

        // Act
        await service.IsActiveAsync(context);

        // Assert
        context.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task IsActiveAsync_InactiveUser_SetsIsActiveFalse()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.NotFound,_handlerMock);
        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildIsActiveContext();

        await service.IsActiveAsync(context);

        
        context.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task IsActiveAsync_FastApiDown_SetsIsActiveFalse()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var service = new UserProfileService(_factoryMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
        var context = ContextHelper.BuildIsActiveContext();

        await service.IsActiveAsync(context);

        context.IsActive.Should().BeFalse();
    }
}