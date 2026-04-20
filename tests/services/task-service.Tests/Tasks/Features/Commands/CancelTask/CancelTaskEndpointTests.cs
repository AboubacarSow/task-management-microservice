using task_service.Tasks.Features.Commands.CancelTask;
using task_service.Tests.Fixtures;

namespace task_service.Tests.Tasks.Features.Commands.CancelTask;

public class CancelTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CancelTaskEndpointTests(WebApplicationFactory<Program> factory)
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

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task PATCH_CancelTask_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<CancelTaskCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);
                   

        
        var response = await _client.PatchAsync(
            $"/api/tasks/{_taskId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_CancelTask_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<CancelTaskCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Unit.Value);
       

        var response = await _client.PatchAsync(
            $"/api/tasks/{_taskId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(
            It.IsAny<CancelTaskCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PATCH_CancelTask_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;
  
        var response = await _client.PatchAsync(
            $"/api/tasks/{_taskId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_CancelTask_ShouldSendCorrectCommand()
    {
        CancelTaskCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<CancelTaskCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                       capturedCommand = (CancelTaskCommand)cmd)
                   .ReturnsAsync(Unit.Value);


      
        var response = await _client.PatchAsync(
            $"/api/tasks/{_taskId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TaskId.Should().Be(_taskId);
        capturedCommand.CurrentUserId.Should().Be(_userId);
    }
}