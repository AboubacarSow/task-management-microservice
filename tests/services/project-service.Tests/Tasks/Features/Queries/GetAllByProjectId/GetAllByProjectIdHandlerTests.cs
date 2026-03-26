using project_service.Projects.Features.Queries.GetProjectById;
using project_service.Projects.Models;
using project_service.Tasks.Dtos;
using project_service.Tasks.Features.Queries.GetAllByProjectId;
using System.Runtime.CompilerServices;

namespace project_service.Tests.Tasks.Features.Queries.GetAllByProjectId;


public class GetAllByProjectIdHandlerTests
{

    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ILogger<GetAllByProjectIdHandler>> _loggerMock = new();

    private readonly GetAllByProjectIdHandler _handler;

    private readonly Project _project = new("Building a web scraper", Guid.NewGuid());
    private readonly Guid _current_user = Guid.NewGuid();

    public GetAllByProjectIdHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,_projectRepoMock.Object,
            _loggerMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Return_Tasks_When_Project_Has_Tasks()
    {
        // Arrange
        _project.AddToPeopleWorking(_current_user);       
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(),_project.Id);
 
        var project_tasks = tasks.Where(t=>t.ProjectId == _project.Id)
                                 .ToList();

        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(_project.Id))
                     .ReturnsAsync(project_tasks);

        var query = new GetAllByProjectIdQuery(_current_user, _project.Id);

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
        var projectId = Guid.NewGuid();
        _project.AddToPeopleWorking(_current_user);
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(), projectId);
        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(_project.Id))
            .ReturnsAsync([]);

        var query = new GetAllByProjectIdQuery(_current_user,_project.Id);

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
        _project.AddToPeopleWorking(_current_user);
        var tasks = FakeTaskData
            .GetTasksForMultipleProjects(Guid.NewGuid(), _project.Id);

        var project_tasks = tasks.Where(t=>t.ProjectId == _project.Id).ToList();
        
        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
                        .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(_project.Id))
            .ReturnsAsync(project_tasks);


        var query = new GetAllByProjectIdQuery(_current_user,_project.Id);

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
        _project.AddToPeopleWorking(_current_user);

        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
                        .ReturnsAsync(_project);
        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(_project.Id))
            .ReturnsAsync([]);

        var query = new GetAllByProjectIdQuery(_current_user,_project.Id);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _projectRepoMock.Verify(r => r.GetByIdAsync(_project.Id), Times.Once);
        _taskRepoMock.Verify(r => r.GetAllByProjectIdAsync(_project.Id), Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Tasks_FoundAsync()
    {
        // Arrange

        _project.AddToPeopleWorking(_current_user);

        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
            .ReturnsAsync(_project);

        _taskRepoMock.Setup(r => r.GetAllByProjectIdAsync(_project.Id))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetAllByProjectIdQuery(_current_user,_project.Id), CancellationToken.None);

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

        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
            .ReturnsAsync(_project);

        var query = new GetAllByProjectIdQuery(currentUserId, _project.Id);

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

        _projectRepoMock.Setup(r => r.GetByIdAsync(_project.Id))
            .ReturnsAsync((Project)null!);

        var query = new GetAllByProjectIdQuery(currentUserId, _project.Id);

        // Act
        Action action = () => _handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult(); ;

        // Assert
         action.Should().Throw<NotFoundException>();
            
    }
}