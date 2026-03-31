

namespace task_service.Tests.Tasks.Features.Commands.AssignTaskTo;

public class AssignTaskToEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public AssignTaskToEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task PATCH_AssignTask_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<AssignTaskToCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);

        var request = new { UserId = Guid.NewGuid() };

        var response = await _client.PatchAsJsonAsync(
            $"/api/tasks/{_taskId}/assign", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_AssignTask_ShouldCallHandler()
    {
        var result = Unit.Value;
        _senderMock.Setup(s => s.Send(It.IsAny<AssignTaskToCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(result);

        var request = new { UserId = Guid.NewGuid() };

        var response = await _client.PatchAsJsonAsync(
            $"/api/tasks/{_taskId}/assign", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(
            It.IsAny<AssignTaskToCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PATCH_AssignTask_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new { UserId = Guid.NewGuid() };

        var response = await _client.PatchAsJsonAsync(
            $"/api/tasks/{_taskId}/assign", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_AssignTask_ShouldSendCorrectCommand()
    {
        AssignTaskToCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<AssignTaskToCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (AssignTaskToCommand)cmd;
                   })
                   .ReturnsAsync(Unit.Value);

        var assignedUser = Guid.NewGuid();

        var request = new { UserId = assignedUser };

        var response = await _client.PatchAsJsonAsync(
            $"/api/tasks/{_taskId}/assign", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TaskId.Should().Be(_taskId);
        capturedCommand.UserId.Should().Be(assignedUser);

        capturedCommand.CurrentUserId.Should()
            .Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }
}