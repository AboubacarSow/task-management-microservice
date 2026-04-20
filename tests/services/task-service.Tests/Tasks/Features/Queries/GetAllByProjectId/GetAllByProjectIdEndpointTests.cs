<<<<<<< HEAD
using Mapster;
using project_service.Tasks.Dtos;
using task_service.Tasks.Features.Queries.GetAllByProjectId;

namespace project_service.Tests.Tasks.Features.Queries.GetAllByProjectId;

public class GetAllByProjectIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetAllByProjectIdEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task GET_Tasks_Should_Return_200_When_Project_Has_Tasks()
    {
        var projectId = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleProjects(projectId,projectId);

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks.Adapt<List<TaskItemDto>>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_Correct_List_When_Project_Has_Tasks()
    {
        var projectId = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleProjects(projectId, projectId);

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks.Adapt<List<TaskItemDto>>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(9);
        body![0].Name.Should().Be(tasks[0].Name);
        body![1].Name.Should().Be(tasks[1].Name);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_200_With_Empty_List_When_No_Tasks()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_401_When_Unauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/tasks/project_id={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Tasks_Should_Use_MediatR_Handler()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        await _client.GetAsync($"/api/tasks/project_id={projectId}");

        _senderMock.Verify(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Tasks_Should_Send_ProjectId_And_UserId_From_Context()
    {
        var projectId = Guid.NewGuid();
        var expectedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        GetAllByProjectIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<List<TaskItemDto>>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetAllByProjectIdQuery)q;
            })
            .ReturnsAsync(new List<TaskItemDto>());

        await _client.GetAsync($"/api/tasks/project_id={projectId}");

        capturedQuery.Should().NotBeNull();
        capturedQuery!.ProjectId.Should().Be(projectId);
        capturedQuery!.CurrentUserId.Should().Be(expectedUserId);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_404_When_Project_NotFound()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(nameof(Project), projectId.ToString()));

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_403_When_User_Not_Authorized()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException("user", "READ_PROJECT"));

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
=======

using task_service.Tasks.Features.Queries.GetAllByProjectId;

namespace task_service.Tests.Tasks.Features.Queries.GetAllByProjectId;

public class GetAllByProjectIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();

    public GetAllByProjectIdEndpointTests(WebApplicationFactory<Program> factory)
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

        _client = _webApplicationFactory.Server.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "fake");
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_200_When_Project_Has_Tasks()
    {
        var projectId = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleProjects(projectId,projectId);

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks.Adapt<List<TaskItemDto>>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_Correct_List_When_Project_Has_Tasks()
    {
        var projectId = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleProjects(projectId, projectId);

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks.Adapt<List<TaskItemDto>>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(9);
        body![0].Name.Should().Be(tasks[0].Name);
        body![1].Name.Should().Be(tasks[1].Name);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_200_With_Empty_List_When_No_Tasks()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");
        var body = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_401_When_Unauthenticated()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/tasks/project_id={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Tasks_Should_Use_MediatR_Handler()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItemDto>());

        await _client.GetAsync($"/api/tasks/project_id={projectId}");

        _senderMock.Verify(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GET_Tasks_Should_Send_ProjectId_And_UserId_From_Context()
    {
        var projectId = Guid.NewGuid();
        var expectedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        GetAllByProjectIdQuery? capturedQuery = null;

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<List<TaskItemDto>>, CancellationToken>((q, _) =>
            {
                capturedQuery = (GetAllByProjectIdQuery)q;
            })
            .ReturnsAsync(new List<TaskItemDto>());

        await _client.GetAsync($"/api/tasks/project_id={projectId}");

        capturedQuery.Should().NotBeNull();
        capturedQuery!.ProjectId.Should().Be(projectId);
        capturedQuery!.CurrentUserId.Should().Be(expectedUserId);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_404_When_Project_NotFound()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Project", projectId.ToString()));

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Tasks_Should_Return_403_When_User_Not_Authorized()
    {
        var projectId = Guid.NewGuid();

        _senderMock.Setup(r => r.Send(It.IsAny<GetAllByProjectIdQuery>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException("user", "READ_PROJECT"));

        var response = await _client.GetAsync($"/api/tasks/project_id={projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
>>>>>>> 05b451b (new_update)
}