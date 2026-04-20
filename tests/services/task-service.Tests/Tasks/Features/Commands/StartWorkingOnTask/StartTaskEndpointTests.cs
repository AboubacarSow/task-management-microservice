using task_service.Tasks.Features.Commands.StartWorkingOnTask;
using task_service.Tests.Fixtures;

namespace task_service.Tests.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public StartTaskEndpointTests(WebApplicationFactory<Program> factory)
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
                }).AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddScoped<IUserContext, FakeUserContext>();
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task POST_StartTask_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<StartTaskCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);

        var response = await _client.PostAsync($"/api/tasks/{_taskId}/start", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task POST_StartTask_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<StartTaskCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);

        var response = await _client.PostAsync($"/api/tasks/{_taskId}/start", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(It.IsAny<StartTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task POST_StartTask_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync($"/api/tasks/{_taskId}/start", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_StartTask_ShouldSendCorrectCommand()
    {
        StartTaskCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<StartTaskCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) => capturedCommand = (StartTaskCommand)cmd)
                   .ReturnsAsync(Unit.Value);

        var response = await _client.PostAsync($"/api/tasks/{_taskId}/start", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TaskId.Should().Be(_taskId);
        capturedCommand.CurrentUserId.Should().Be(_userId);
    }
}