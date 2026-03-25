using project_service.Tasks.Dtos;
using project_service.Tasks.Features.Queries.GetTasksByOwnerId;

namespace project_service.Tests.Tasks.Features.Queries.GetTasksByOwnerId;

public class GetTasksByOwnerIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetTasksByOwnerIdEndpointTests(WebApplicationFactory<Program> factory)
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

        _client = _webApplicationFactory.Server.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_200_When_Owner_Has_Tasks()
    {
        var owner = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleUsers(owner,owner);

        _senderMock.Setup(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var response = await _client.GetAsync("/api/tasks/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_Correct_List_When_Owner_Has_TasksAsync()
    {
        var owner = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleUsers(owner, Guid.NewGuid());

        var expectedTasks = tasks.Where(t=>t.CreatedByUser == owner).ToList();

        _senderMock.Setup(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTasks);

        // Act
        var response = await _client.GetAsync("/api/tasks/me");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(4);
        body![0].Name.Should().Be(expectedTasks[0].Name);
        body![1].Name.Should().Be(expectedTasks[1].Name);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_200_With_Empty_List_When_No_Tasks_FoundAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        // Act
        var response = await _client.GetAsync("/api/tasks/me");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_401_When_UnauthenticatedAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/tasks/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Tasks_Should_Use_MediatR_HandlerAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        // Act
        await _client.GetAsync("/api/tasks/me");

        // Assert
        _senderMock.Verify(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Tasks_Should_Send_OwnerId_From_UserContextAsync()
    {
        // Arrange
        var expectedOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        GetTasksByOwnerIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetTasksByOwnerIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<IEnumerable<TaskItemDto>>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetTasksByOwnerIdQuery)q;
            })
            .ReturnsAsync(new List<TaskItemDto>());

        // Act
        await _client.GetAsync("/api/tasks/me");

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.CurrentUserId.Should().Be(expectedOwnerId); 
    }

}