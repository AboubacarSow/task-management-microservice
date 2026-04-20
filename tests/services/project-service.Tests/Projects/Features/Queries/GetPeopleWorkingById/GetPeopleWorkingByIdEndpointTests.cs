<<<<<<< HEAD
using project_service.Projects.Features.Queries.GetPeopleWorkingById;


namespace project_service.Tests.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();

    public GetPeopleWorkingByIdEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
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

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldReturn200()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _senderMock.Verify(s => s.Send(
            It.IsAny<GetPeopleWorkingByIdQuery>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldSendCorrectQuery()
    {
        GetPeopleWorkingByIdQuery? capturedQuery = null;

        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<List<Guid>>, CancellationToken>((q, _) =>
                   {
                       capturedQuery = (GetPeopleWorkingByIdQuery)q;
                   })
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        capturedQuery.Should().NotBeNull();
        capturedQuery!.ProjectId.Should().Be(_projectId);
    }
=======
using project_service.Projects.Features.Queries.GetPeopleWorkingById;
using project_service.Tests.Fixtures;
using shared.Utilities;


namespace project_service.Tests.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();

    public GetPeopleWorkingByIdEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
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
                services.AddScoped<IUserContext, FakeUserContext>();
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldReturn200()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _senderMock.Verify(s => s.Send(
            It.IsAny<GetPeopleWorkingByIdQuery>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_PeopleWorking_ShouldSendCorrectQuery()
    {
        GetPeopleWorkingByIdQuery? capturedQuery = null;

        _senderMock.Setup(s => s.Send(It.IsAny<GetPeopleWorkingByIdQuery>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<List<Guid>>, CancellationToken>((q, _) =>
                   {
                       capturedQuery = (GetPeopleWorkingByIdQuery)q;
                   })
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/people");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        capturedQuery.Should().NotBeNull();
        capturedQuery!.ProjectId.Should().Be(_projectId);
    }
>>>>>>> 05b451b (new_update)
}