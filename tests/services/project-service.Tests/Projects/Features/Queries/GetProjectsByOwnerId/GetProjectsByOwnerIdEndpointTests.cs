<<<<<<< HEAD
using project_service.Projects.Features.Queries.GetProjectsByOwnerId;

namespace project_service.Tests.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetProjectsByOwnerIdEndpointTests(WebApplicationFactory<Program> factory)
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

        _client = _webApplicationFactory.Server.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_Projects_Should_Return_200_When_Owner_Has_ProjectsAsync()
    {
        var projects = FakeProjectData.GetProjectsDto(Guid.NewGuid());

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var response = await _client.GetAsync("/api/projects/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Projects_Should_Return_Correct_List_When_Owner_Has_ProjectsAsync()
    {
        var expectedProjects = FakeProjectData.GetProjectsDto(Guid.NewGuid());

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProjects);

        // Act
        var response = await _client.GetAsync("/api/projects/me");
        var body = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(4);
        body![0].Name.Should().Be(expectedProjects[0].Name);
        body![1].Name.Should().Be(expectedProjects[1].Name);
    }

    [Fact]
    public async Task GET_Projects_Should_Return_200_With_Empty_List_When_No_Projects_FoundAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var response = await _client.GetAsync("/api/projects/me");
        var body = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_Projects_Should_Return_401_When_UnauthenticatedAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/projects/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Projects_Should_Use_MediatR_HandlerAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectDto>());

        // Act
        await _client.GetAsync("/api/projects/me");

        // Assert
        _senderMock.Verify(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Projects_Should_Send_OwnerId_From_UserContextAsync()
    {
        // Arrange
        // FakeAuthHandler sets the user ID to this value
        var expectedOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        GetProjectsByOwnerIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<IEnumerable<ProjectDto>>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetProjectsByOwnerIdQuery)q;
            })
            .ReturnsAsync(new List<ProjectDto>());

        // Act
        await _client.GetAsync("/api/projects/me");

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.OwnerId.Should().Be(expectedOwnerId); // comes from IUserContext not route
    }

=======
using Elasticsearch.Net.Specification.IndicesApi;
using project_service.Projects.Features.Queries.GetProjectsByOwnerId;
using project_service.Tests.Fixtures;
using shared.Utilities;

namespace project_service.Tests.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetProjectsByOwnerIdEndpointTests(WebApplicationFactory<Program> factory)
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

        _client = _webApplicationFactory.Server.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_Projects_Should_Return_200_When_Owner_Has_ProjectsAsync()
    {
        var ownerId = Guid.NewGuid();
        var projects = FakeProjectData.GetProjectsDto(ownerId);

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var response = await _client.GetAsync("/api/projects/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Projects_Should_Return_Correct_List_When_Owner_Has_ProjectsAsync()
    {
        var expectedProjects = FakeProjectData.GetProjectsDto(Guid.NewGuid());

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProjects);

        // Act
        var response = await _client.GetAsync("/api/projects/me");
        var body = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(4);
        body![0].Name.Should().Be(expectedProjects[0].Name);
        body![1].Name.Should().Be(expectedProjects[1].Name);
    }

    [Fact]
    public async Task GET_Projects_Should_Return_200_With_Empty_List_When_No_Projects_FoundAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var response = await _client.GetAsync("/api/projects/me");
        var body = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_Projects_Should_Return_401_When_UnauthenticatedAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/projects/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Projects_Should_Use_MediatR_HandlerAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectDto>());

        // Act
        await _client.GetAsync("/api/projects/me");

        // Assert
        _senderMock.Verify(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Projects_Should_Send_OwnerId_From_UserContextAsync()
    {
        // Arrange
        // FakeAuthHandler sets the user ID to this value
        var expectedOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        GetProjectsByOwnerIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetProjectsByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<IEnumerable<ProjectDto>>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetProjectsByOwnerIdQuery)q;
            })
            .ReturnsAsync(new List<ProjectDto>());

        // Act
        await _client.GetAsync("/api/projects/me");

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.OwnerId.Should().Be(expectedOwnerId); // comes from IUserContext not route
    }

>>>>>>> 05b451b (new_update)
}