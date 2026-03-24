using project_service.Tasks.Features.Commands.UnAssignTask;

namespace project_service.Tests.Tasks.Features.Commands.UnAssignTask;

public class UnassignTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();
    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public UnassignTaskEndpointTests(WebApplicationFactory<Program> factory)
    {
        var app = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(_senderMock.Object);

                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddScoped<IUserContext, HttpUserContext>();
            });
        });

        _client = app.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task PATCH_Unassign_ShouldReturn204()
    {
        _senderMock.Setup(x => x.Send(It.IsAny<UnAssignTaskCommand>(), default))
                   .ReturnsAsync(Unit.Value);

        var res = await _client.PatchAsync($"/api/tasks/{_taskId}/unassign", null);

        res.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_Unassign_ShouldCallHandler()
    {
        _senderMock.Setup(x => x.Send(It.IsAny<UnAssignTaskCommand>(), default))
                   .ReturnsAsync(Unit.Value);

        await _client.PatchAsync($"/api/tasks/{_taskId}/unassign", null);

        _senderMock.Verify(x => x.Send(It.IsAny<UnAssignTaskCommand>(), default), Times.Once);
    }

    [Fact]
    public async Task PATCH_Unassign_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var res = await _client.PatchAsync($"/api/tasks/{_taskId}/unassign", null);

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_AssignTask_ShouldSendCorrectCommand()
    {
        UnAssignTaskCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<UnAssignTaskCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (UnAssignTaskCommand)cmd;
                   })
                   .ReturnsAsync(Unit.Value);

        var assignedUser = Guid.NewGuid();


        var response = await _client.PatchAsync(
            $"/api/tasks/{_taskId}/unassign", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TaskId.Should().Be(_taskId);
        capturedCommand.CurrentUserId.Should().Be(assignedUser);

        capturedCommand.CurrentUserId.Should()
            .Be(_userId);
    }
}