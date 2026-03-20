using System.Net;
using authentication_service.Tests.Helpers;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace authentication_service.Tests.Services;


public class UserProfileServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;

    public UserProfileServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("http://user-service:8000")
        };
    }

    [Fact]
    public async Task GetProfileDataAsync_ValidUser_SetsEmailClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should()
            .Contain(c => c.Type == "email" && c.Value == "john@example.com");
    }

    [Fact]
    public async Task GetProfileDataAsync_ValidUser_SetsUsernameClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        
        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should().Contain(c => c.Type == "preferred_username" && c.Value == "johndoe");
    }

    [Fact]
    public async Task GetProfileDataAsync_ValidUser_SetsGivenNameClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK, _handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should().Contain(c => c.Type == "given_name" && c.Value == "John");
    }

    [Fact]
    public async Task GetProfileDataAsync_ValidUser_SetsFamilyNameClaim()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock, PayloadHelper.ValidUserPayload());
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        context.IssuedClaims
            .Should().Contain(c => c.Type == "family_name" && c.Value == "Doe");
    }

    [Fact]
    public async Task GetProfileDataAsync_UserNotFound_NoClaimsIssued()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.NotFound,_handlerMock);
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        
        context.IssuedClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProfileDataAsync_FastApiDown_NoClaimsIssued()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildProfileContext();

        await service.GetProfileDataAsync(context);

        
        context.IssuedClaims.Should().BeEmpty();
    }

    [Fact]
    public async Task IsActiveAsync_ActiveUser_SetsIsActiveTrue()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.OK,_handlerMock);
        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildIsActiveContext();

        await service.IsActiveAsync(context);

        context.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task IsActiveAsync_InactiveUser_SetsIsActiveFalse()
    {
        ResponseHelper.SetupResponse(HttpStatusCode.NotFound,_handlerMock);
        var service = new UserProfileService(_httpClient);
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

        var service = new UserProfileService(_httpClient);
        var context = ContextHelper.BuildIsActiveContext();

        await service.IsActiveAsync(context);

        context.IsActive.Should().BeFalse();
    }
}