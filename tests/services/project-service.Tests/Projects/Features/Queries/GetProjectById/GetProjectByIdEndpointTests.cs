using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using project_service.Data.Utilities;
using project_service.Projects.Dtos;
using project_service.Projects.Features.Queries.GetProjectById;
using project_service.Projects.Models;
using project_service.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace project_service.Tests.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetProjectByIdEndpointTests(WebApplicationFactory<Program> factory)
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
                services.AddScoped<IUserContext, HttpUserContext>();
            });
        });

        _client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_Project_Should_Return_200_When_Project_ExistsAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectDto(
                projectId,
                "Building a web scraper",
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(3),
                "Using TDD approach",
                ProjectStatus.Active,
                Guid.NewGuid()));

        // Act
        var response = await _client.GetAsync($"/api/projects/{projectId}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Project_Should_Return_Correct_Body_When_Project_Exists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var expectedProject = new ProjectDto(
                projectId,
                "Building a web scraper",
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(3),
                "Using TDD approach",
                ProjectStatus.Active,
                Guid.NewGuid());

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProject);

        // Act
        var response = await GetProjectAsync(projectId);
        var body = await response.Content.ReadFromJsonAsync<ProjectDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Id.Should().Be(projectId);
        body.Name.Should().Be(expectedProject.Name);
        body.Description.Should().Be(expectedProject.Description);
    }

    [Fact]
    public async Task GET_Project_Should_Return_404_When_Project_Not_FoundAsync()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()))!
            .ReturnsAsync((ProjectDto?)null);

        var response = await GetProjectAsync(projectId);


        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Project_Should_Return_401_When_UnauthenticatedAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var projectId = Guid.NewGuid();

        var response = await GetProjectAsync(projectId);


        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Project_Should_Use_MediatR_HandlerAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectDto(
                projectId,
                "Building a web scraper",
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(3),
                "Using TDD approach",
                ProjectStatus.Active,
                Guid.NewGuid()));

        // Act
        await GetProjectAsync(projectId);

        // Assert
        _senderMock.Verify(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Project_Should_Send_Correct_QueryAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        GetProjectByIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<ProjectDto?>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetProjectByIdQuery)q;
            })
            .ReturnsAsync(new ProjectDto(
                projectId,
                "Building a web scraper",
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(3),
                "Using TDD approach",
                ProjectStatus.Active,
                Guid.NewGuid()));

        // Act
        await GetProjectAsync(projectId);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.Id.Should().Be(projectId);
    }

    private async Task<HttpResponseMessage> GetProjectAsync(Guid projectId) {
        return await _client.GetAsync(new Uri($"api/projects/{projectId}", UriKind.Relative));

    }
}