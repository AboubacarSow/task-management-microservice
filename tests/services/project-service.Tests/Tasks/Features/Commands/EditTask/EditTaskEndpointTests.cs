using project_service.Tasks.Dtos;
using project_service.Tasks.Features.Commands.EditTask;

namespace project_service.Tests.Tasks.Features.Commands.EditTask;

public class EditTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();
    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public EditTaskEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task PATCH_Edit_ShouldSendCorrectCommand()
    {
        EditTaskCommand? captured = null;

        _senderMock.Setup(x => x.Send(It.IsAny<EditTaskCommand>(), default))
            .Callback<IRequest<TaskItemDto>, CancellationToken>((cmd, _) =>
                captured = (EditTaskCommand)cmd)
            .ReturnsAsync(It.IsAny<TaskItemDto>);

        var due = DateTime.UtcNow.AddDays(2);

        await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}",
            new
            {
                Title = "new",
                DueAt = due,
                Description = "desc"
            });

        captured.Should().NotBeNull();
        captured!.TaskId.Should().Be(_taskId);
        captured.CurrentUserId.Should().Be(_userId);
        captured.Title.Should().Be("new");
        captured.Description.Should().Be("desc");
    }

    [Fact]
    public async Task PATCH_Edit_ShouldReturn401_WhenUnauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var due = DateTime.UtcNow.AddDays(2);
        var res = await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}", new
        {
            Title = "new",
            DueAt = due,
            Description = "desc"
        });

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PATCH_Edit_ShouldReturn200_WithDto()
    {
        var taskDto = new TaskItemDto(
                Id:Guid.NewGuid(),
                Name: "Setup database schema",
                ProjectId: Guid.NewGuid(),
                CreatedByUser: Guid.NewGuid(),
                AssignedToUser: null,
                Priority: TaskPriority.Medium,
                Status: project_service.Tasks.Models.TaskStatus.InProgress,
                DueAt: DateTime.UtcNow.AddDays(5),
                Description: null,
                CreatedAt: DateTime.UtcNow,
                LastUpdatedAt: DateTime.UtcNow,
                Note: null
            );

        _senderMock.Setup(x => x.Send(It.IsAny<EditTaskCommand>(), default))
                .ReturnsAsync(taskDto);

        var res = await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}",
            new
            {
                Title = "new",
                DueAt = DateTime.UtcNow,
                Description = "desc"
            });

        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await res.Content.ReadFromJsonAsync<TaskItemDto>();

        result.Should().NotBeNull();
        result!.DueAt.Should().Be(taskDto.DueAt);
    }
}