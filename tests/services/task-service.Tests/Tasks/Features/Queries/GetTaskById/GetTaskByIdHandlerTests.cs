using project_service.Tasks.Features.Queries.GetTaskById;

namespace project_service.Tests.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ILogger<GetTaskByIdHandler>> _loggerMock = new();

    private readonly GetTaskByIdHandler _handler ;

    public GetTaskByIdHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,
                    _projectRepoMock.Object, 
                    _loggerMock.Object);

    }

    [Fact]
    public async Task Handle_Should_Return_TaskItemDto_When_Task_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(userId);
        var task = new TaskItem("task", project.Id, userId);


        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id))
            .ReturnsAsync(task);
        
        _projectRepoMock.Setup(r=>r.GetByIdAsync(project.Id))
        .ReturnsAsync(project);

        var query = new GetTaskByIdQuery(userId, task.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(task.Status);
        result.Name.Should().Be(task.Name);
    }

    [Fact]
    public async Task Handle_Should_Throw_Forbbiden_When_Requester_Not_InPeople()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(owner);
        var task = new TaskItem("task", project.Id, owner);


        _taskRepoMock.Setup(c=>c.GetByIdAsync(task.Id))
                .ReturnsAsync(task);
        _projectRepoMock.Setup(c =>c.GetByIdAsync(project.Id))
        .ReturnsAsync(project);

        var action = ()=> _handler.Handle(new GetTaskByIdQuery(outsider,task.Id), 
                                        CancellationToken.None);
        
        await action.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFound_When_Task_Null()
    {
        var taskId = Guid.NewGuid();
        var current_user = Guid.NewGuid();
        _taskRepoMock.Setup(c=>c.GetByIdAsync(It.IsAny<Guid>()))
        .ReturnsAsync((TaskItem)null!);


        var action = ()=> _handler.Handle(new GetTaskByIdQuery(current_user,taskId), 
                                        CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
}