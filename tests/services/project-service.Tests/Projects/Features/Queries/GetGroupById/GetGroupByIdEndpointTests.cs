using project_service.Projects.Features.Queries.GetGroupById;
using project_service.Tests.Fixtures;
using shared.Utilities;

namespace project_service.Tests.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();

    public GetGroupByIdEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task GET_Group_ShouldReturn200()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<Guid>());

        var response = await _client.GetAsync($"/api/projects/{_projectId}/group");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Group_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<Guid>());

        var response = await _client.GetAsync($"/api/projects/{_projectId}/group");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _senderMock.Verify(s => s.Send(
            It.IsAny<GetGroupByIdQuery>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GET_Group_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/projects/{_projectId}/group");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Group_ShouldSendCorrectQuery()
    {
        GetGroupByIdQuery? capturedQuery = null;

        _senderMock.Setup(s => s.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<List<Guid>>, CancellationToken>((q, _) =>
                   {
                       capturedQuery = (GetGroupByIdQuery)q;
                   })
                   .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/projects/{_projectId}/group");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        capturedQuery.Should().NotBeNull();
        capturedQuery!.ProjectId.Should().Be(_projectId);
    }
}