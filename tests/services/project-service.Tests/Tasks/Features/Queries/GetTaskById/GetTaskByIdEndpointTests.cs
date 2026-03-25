using project_service.Tasks.Dtos;
using project_service.Tasks.Features.Queries.GetTaskById;

namespace project_service.Tests.Tasks.Features.Queries.GetTaskById;

public class GetTaskByIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetTaskByIdEndpointTests(WebApplicationFactory<Program> factory)
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
            AllowAutoRedirect = false,
        });
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }
    [Fact]
    public async Task GET_Task_Should_Return_200_When_Task_Exists()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetTaskByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync( new TaskItemDto(
                Id: Guid.NewGuid(),
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
            ));

        // Act
        var response = await _client.GetAsync($"/api/tasks/{taskId}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Task_Should_Return_Correct_Body_When_Task_Exists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedProject = ( new TaskItemDto(
                Id: taskId,
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
            ));

        _senderMock.Setup(r => r.Send(It.IsAny<GetTaskByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProject);

        // Act
        var response = await GetTaskByIdAsync(taskId);
        var body = await response.Content.ReadFromJsonAsync<TaskItemDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Id.Should().Be(taskId);
        body.Name.Should().Be(expectedProject.Name);
        body.Priority.Should().Be(expectedProject.Priority);
    }

    [Fact]
    public async Task GET_Task_Should_Use_MediatR_Handler()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetTaskByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync( new TaskItemDto(
                Id: taskId,
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
            ));

        // Act
        await GetTaskByIdAsync(taskId);

        // Assert
        _senderMock.Verify(r => r.Send(It.IsAny<GetTaskByIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Task_Should_Send_Correct_Query()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        GetTaskByIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetTaskByIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<TaskItemDto?>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetTaskByIdQuery)q;
            })
            .ReturnsAsync( new TaskItemDto(
                Id: Guid.NewGuid(),
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
            ));

        // Act
        await GetTaskByIdAsync(taskId);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.TaskId.Should().Be(taskId);
    }

    private async Task<HttpResponseMessage> GetTaskByIdAsync(Guid taskId) {
        return await _client.GetAsync(new Uri($"api/tasks/{taskId}", UriKind.Relative));

    }
}