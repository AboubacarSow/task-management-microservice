<<<<<<< HEAD

using project_service.Tasks.Features.Commands.CreateTaskItem;

namespace project_service.Tests.Tasks.Features.Commands.CreateTaskItem;

public class CreateTaskItemHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly Mock<ILogger<CreateTaskItemHandler>> _logger = new();

    private readonly CreateTaskItemHandler _handler;

    public CreateTaskItemHandlerTests()
    {
        _handler = new CreateTaskItemHandler(
            _taskRepo.Object,
            _projectRepo.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Should_Create_Task_When_User_In_Group()
    {
        var userId = Guid.NewGuid();
        var project =FakeProjectData.BuildProject(userId);
        project.AddUserToGroup(userId);

        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskItemCommand(userId,project.Id, "New Task");

        Guid result = await _handler.Handle(command, CancellationToken.None);

        _taskRepo.Verify(x => x.AddAsync(It.Is<TaskItem>(
            t => t.Name == "New Task" &&
                 t.ProjectId == project.Id &&
                 t.CreatedByUser == userId)), Times.Once);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_Throw_When_Project_Not_Found()
    {
        
        _projectRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Project?)null);

        var command = new CreateTaskItemCommand(Guid.NewGuid(),Guid.NewGuid(), "Task");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, default));
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_In_Group()
    {
        var userId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(userId);

        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskItemCommand(Guid.NewGuid(),project.Id, "Task");

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, default));
    }
}
=======
using project_grpc_server;
using task_service.Tasks.Features.Commands.CreateTaskItem;
using task_service.Tasks.Grpc.Client;

namespace task_service.Tests.Tasks.Features.Commands.CreateTaskItem;

public class CreateTaskItemHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectClient> _projectClientMock = new();
    private readonly Mock<ILogger<CreateTaskItemHandler>> _logger = new();

    private readonly CreateTaskItemHandler _handler;

    public CreateTaskItemHandlerTests()
    {
        _handler = new CreateTaskItemHandler(
            _taskRepo.Object,
            _projectClientMock.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Should_Create_Task_When_User_In_Group()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var group = new List<Guid> { userId };
        var project = FakeProjectModelData.BuildProjectModel(projectId, Guid.NewGuid(),
            group.AsEnumerable());

        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskItemCommand(userId,projectId, "New Task");

        Guid result = await _handler.Handle(command, CancellationToken.None);

        _taskRepo.Verify(x => x.AddAsync(It.Is<TaskItem>(
            t => t.Name == "New Task" &&
                 t.ProjectId == projectId &&
                 t.CreatedByUser == userId)), Times.Once);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_Throw_When_Project_Not_Found()
    {
        
        _projectClientMock.Setup(x => x.GetProjectAsync(It.IsAny<string>()))!
            .ReturnsAsync((ProjectModel?)null);

        var command = new CreateTaskItemCommand(Guid.NewGuid(),Guid.NewGuid(), "Task");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, default));
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_In_Group()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var group = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var project = FakeProjectModelData.BuildProjectModel(projectId, Guid.NewGuid(),
            group.AsEnumerable());

        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);

        var command = new CreateTaskItemCommand(Guid.NewGuid(),projectId, "Task");

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, default));
    }
}
>>>>>>> 05b451b (new_update)
