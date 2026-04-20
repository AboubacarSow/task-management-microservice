using task_service.Tasks.Features.Commands.CreateTaskItem;
using task_service.Tests.Fixtures;

namespace task_service.Tests.Tasks.Features.Commands.CreateTaskItem;

public class CreateTaskItemEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();


    public CreateTaskItemEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task POST_Task_ShouldReturn201()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<CreateTaskItemCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Guid.NewGuid());

        var request = new
        {
            ProjectId = Guid.NewGuid(),
            Title = "New Task"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_Task_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<CreateTaskItemCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Guid.NewGuid());

        var request = new
        {
            ProjectId = Guid.NewGuid(),
            Title = "Task"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _senderMock.Verify(s => s.Send(
            It.IsAny<CreateTaskItemCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task POST_Task_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            ProjectId = Guid.NewGuid(),
            Title = "Task"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_Task_ShouldSendCorrectCommand()
    {
        CreateTaskItemCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<CreateTaskItemCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Guid>, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (CreateTaskItemCommand)cmd;
                   })
                   .ReturnsAsync(Guid.NewGuid());

        var projectId = Guid.NewGuid();
        var title = "Build Task Feature";

        var request = new
        {
            ProjectId = projectId,
            Title = title
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.ProjectId.Should().Be(projectId);
        capturedCommand.Title.Should().Be(title);

        capturedCommand.CurrentUserId.Should()
            .Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }
}