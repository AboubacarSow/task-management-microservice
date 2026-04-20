<<<<<<< HEAD
using project_service.Projects.Features.Commands.EditProject;

namespace project_service.Tests.Projects.Features.Commands.EditProject;

public class EditProjectEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public EditProjectEndpointTests(WebApplicationFactory<Program> factory)
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
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task PATCH_Project_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

        var request = new
        {
            Description = "Updated description",
            DueAt = DateTime.UtcNow.AddDays(5)
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_Project_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

        var request = new
        {
            Description = "Updated description",
            DueAt = DateTime.UtcNow.AddDays(5)
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PATCH_Project_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            Description = "Some description"
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_Project_ShouldSendCorrectCommand()
    {
        EditProjectCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (EditProjectCommand)cmd;
                   })
                   .Returns(Task.CompletedTask);

        var description = "New description";
        var dueAt = DateTime.UtcNow.AddDays(3);

        var request = new
        {
            Description = description,
            DueAt = dueAt
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.ProjectId.Should().Be(_projectId);
        capturedCommand.UserId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        capturedCommand.Description.Should().Be(description);
        capturedCommand.DueAt.Should().Be(dueAt);
    }
=======
using project_service.Projects.Features.Commands.EditProject;
using project_service.Tests.Fixtures;
using shared.Utilities;

namespace project_service.Tests.Projects.Features.Commands.EditProject;

public class EditProjectEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    private readonly Guid _projectId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public EditProjectEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task PATCH_Project_ShouldReturn204()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(It.IsAny<Unit>);

        var request = new
        {
            Description = "Updated description",
            DueAt = DateTime.UtcNow.AddDays(5)
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_Project_ShouldCallHandler()
    {
        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(It.IsAny<Unit>);

        var request = new
        {
            Description = "Updated description",
            DueAt = DateTime.UtcNow.AddDays(5)
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _senderMock.Verify(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PATCH_Project_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            Description = "Some description"
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_Project_ShouldSendCorrectCommand()
    {
        EditProjectCommand? capturedCommand = null;

        _senderMock.Setup(s => s.Send(It.IsAny<EditProjectCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                   {
                       capturedCommand = (EditProjectCommand)cmd;
                   })
                   .ReturnsAsync(It.IsAny<Unit>);

        var description = "New description";
        var dueAt = DateTime.UtcNow.AddDays(3);

        var request = new
        {
            Description = description,
            DueAt = dueAt
        };

        var response = await _client.PatchAsJsonAsync($"/api/projects/{_projectId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.ProjectId.Should().Be(_projectId);
        capturedCommand.UserId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        capturedCommand.Description.Should().Be(description);
        capturedCommand.DueAt.Should().Be(dueAt);
    }
>>>>>>> 05b451b (new_update)
}