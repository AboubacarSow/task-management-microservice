using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using project_service.Commons.Behaviors;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Projects.Features.Commands.EditProjectState;
using project_service.Projects.Models;
using project_service.Tests.Fixtures;
using project_service.Tests.Helpers;
using shared.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace project_service.Tests.Projects.Features.Commands.EditProjectState;

public class EditProjectStateEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();

    private readonly Guid _projectId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public EditProjectStateEndpointTests(WebApplicationFactory<Program> factory)
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
                }).AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddScoped<IUserContext, FakeUserContext>();
            });
        });

        _client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
    }


    [Fact]
    public async Task PUT_ProjectState_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectStateCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(It.IsAny<Unit>);

        var request = new
        {
            Status = ProjectStatus.Completed
        };

        var response = await _client.PutAsJsonAsync($"/api/projects/{_projectId}/state", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task PUT_ProjectState_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectStateCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(It.IsAny<Unit>);

        var request = new
        {
            Status = ProjectStatus.OnHold
        };

        var response = await _client.PutAsJsonAsync($"/api/projects/{_projectId}/state", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(It.IsAny<EditProjectStateCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }




    [Fact]
    public async Task PUT_ProjectState_ShouldReturn400_WhenStatusInvalid()
    {
        var project = FakeProjectData.BuildProject(_ownerId);
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(_projectId))
               .ReturnsAsync(project);
        var factory = _webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ISender>();
                services.AddSingleton(_projectRepositoryMock.Object);
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblyContaining<EditProjectStateCommand>();
                    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                });
                services.AddValidatorsFromAssemblyContaining<EditProjectStateCommandValidator>();
            });
        });

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");

        var request = new
        {
            Status = (ProjectStatus)999
        };

        var response = await client.PutAsJsonAsync($"/api/projects/{_projectId}/state", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task PUT_ProjectState_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            Status = ProjectStatus.Active
        };

        var response = await _client.PutAsJsonAsync($"/api/projects/{_projectId}/state", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}