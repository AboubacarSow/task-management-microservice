using project_grpc_server;
using task_service.Tasks.Features.Queries.GetAllByProjectId;
using task_service.Tasks.Grpc.Client;


namespace task_service.Tests.Tasks.Features.Queries.GetAllByProjectId;


public class GetAllByProjectIdHandlerTests
{

    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectClient> _projectClientMock = new();
    private readonly Mock<ILogger<GetAllByProjectIdHandler>> _loggerMock = new();

    private readonly GetAllByProjectIdHandler _handler;
    

    private readonly ProjectModel _project = FakeProjectModelData.BuildProjectModel(Guid.NewGuid(), Guid.NewGuid());
    private readonly Guid _current_user = Guid.NewGuid();

    public GetAllByProjectIdHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,_projectClientMock.Object,
            _loggerMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Return_Tasks_When_Project_Has_Tasks()
    {
        // Arrange
        _project.PeopleWorking.Add(_current_user.ToString());
        var projectId = Guid.Parse(_project.Id);
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(),projectId);
 
        var project_tasks = tasks.Where(t=>t.ProjectId == projectId)
                                 .ToList();

        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(projectId))
                     .ReturnsAsync(project_tasks);

        var query = new GetAllByProjectIdQuery(_current_user, projectId);

        // Act
        var result = await _handler
            .Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_Project_Has_No_Tasks()
    {
        // Arrange
        var projectId = Guid.Parse(_project.Id);
        _project.PeopleWorking.Add(_current_user.ToString());
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(), projectId);
        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(projectId))
            .ReturnsAsync([]);

        var query = new GetAllByProjectIdQuery(_current_user,projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Map_All_Fields_CorrectlyAsync()
    {
        // Arrange
        var AssignedUser = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var projectId = Guid.Parse(_project.Id);
        _project.PeopleWorking.Add(_current_user.ToString());
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(), projectId);

        var project_tasks = tasks.Where(t=>t.ProjectId == projectId).ToList();
        
        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(projectId))
            .ReturnsAsync(project_tasks);


        var query = new GetAllByProjectIdQuery(_current_user,projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var mappedProject = result.First();
        mappedProject.Id.Should().Be(project_tasks[0].Id);
        mappedProject.Name.Should().Be(project_tasks[0].Name);
        mappedProject.Description.Should().Be(project_tasks[0].Description);
        mappedProject.CreatedAt.Should().Be(project_tasks[0].CreatedAt);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_OnceAsync()
    {
        // Arrange
        var projectId = Guid.Parse(_project.Id);
        _project.PeopleWorking.Add(_current_user.ToString());
        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
                        .ReturnsAsync(_project);
        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(projectId))
            .ReturnsAsync([]);

        var query = new GetAllByProjectIdQuery(_current_user,projectId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _projectClientMock.Verify(r => r.GetProjectAsync(_project.Id), Times.Once);
        _taskRepoMock.Verify(r => r.GetAllByProjectIdAsync(projectId), Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Tasks_FoundAsync()
    {
        // Arrange

        var projectId = Guid.Parse(_project.Id);
        _project.PeopleWorking.Add(_current_user.ToString());
        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
            .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(projectId))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetAllByProjectIdQuery(_current_user,projectId), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(_project.Id.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowForbiddenException_When_CurrentUser_Not_InPeopleWorking()
    {
        // Arrange

        var currentUserId = Guid.NewGuid();
        var projectId = Guid.Parse(_project.Id);
        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
            .ReturnsAsync(_project);

        var query = new GetAllByProjectIdQuery(currentUserId, projectId);

        // Act
        Action action = () => _handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult();

        // Assert
        action.Should().Throw<ForbiddenException>()
            .WithMessage($"User :{currentUserId} is not authorize to perform the [READ_PROJECT] operation");
    }
    [Fact]
    public void Handle_ShouldThrowNotFoundException_When_Project_Not_Found()
    {
        // Arrange

        var currentUserId = Guid.NewGuid();
        var projectId = Guid.Parse(_project.Id);

        _projectClientMock.Setup(r => r.GetProjectAsync(_project.Id))
            .ReturnsAsync((ProjectModel)null!);

        var query = new GetAllByProjectIdQuery(currentUserId, projectId);

        // Act
        Action action = () => _handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult(); ;

        // Assert
         action.Should().Throw<NotFoundException>();
            
    }
}