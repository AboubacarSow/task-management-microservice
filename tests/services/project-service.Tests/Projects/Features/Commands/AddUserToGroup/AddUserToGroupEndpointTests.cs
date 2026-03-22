using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using project_service.Data.Utilities;
using project_service.Projects.Features.Commands.AddUserToGroup;
using project_service.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace project_service.Tests.Projects.Features.Commands.AddUserToGroup;

public class AddUserToGroupEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();
    private readonly Guid _currentUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public AddUserToGroupEndpointTests(WebApplicationFactory<Program> factory)
    {
        _webApplicationFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(_senderMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddScoped<IUserContext, HttpUserContext>();
            });
        });

        _client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task POST_AddUserToGroup_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<AddUserToGroupCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);

        var request = new
        {
            UserId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync($"/api/projects/{_projectId}/group", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task POST_AddUserToGroup_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<AddUserToGroupCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);

        var request = new
        {
            UserId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync($"/api/projects/{_projectId}/group", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(
            It.IsAny<AddUserToGroupCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task POST_AddUserToGroup_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            UserId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync($"/api/projects/{_projectId}/group", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_AddUserToGroup_ShouldSendCorrectCommand()
    {
        AddUserToGroupCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<AddUserToGroupCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (AddUserToGroupCommand)cmd;
                   })
                   .ReturnsAsync(Unit.Value);

        var userId = Guid.NewGuid();

        var request = new
        {
            TargetUserId = userId
        };

        var response = await _client.PostAsJsonAsync($"/api/projects/{_projectId}/group", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.ProjectId.Should().Be(_projectId);
        capturedCommand.TargetUserId.Should().Be(userId);
    }
}