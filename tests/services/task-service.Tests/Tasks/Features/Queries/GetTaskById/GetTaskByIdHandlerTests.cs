using project_grpc_server;
using task_service.Tasks.Features.Queries.GetTaskById;
using task_service.Tasks.Grpc.Client;

namespace task_service.Tests.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectClient> _projectClientMock = new();
    private readonly Mock<ILogger<GetTaskByIdHandler>> _loggerMock = new();

    private readonly GetTaskByIdHandler _handler ;
    private readonly ProjectModel _project = FakeProjectModelData.BuildProjectModel(Guid.NewGuid(), Guid.NewGuid());
    private readonly Guid _current_user = Guid.NewGuid();
    public GetTaskByIdHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,
                    _projectClientMock.Object, 
                    _loggerMock.Object);

    }

    [Fact]
    public async Task Handle_Should_Return_TaskItemDto_When_Task_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.Parse(_project.Id);
        var task = new TaskItem("task", projectId, userId);

        _project.PeopleWorking.Add(userId.ToString());

        _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id))
            .ReturnsAsync(task);
        
        _projectClientMock.Setup(r=>r.GetProjectAsync(_project.Id))
        .ReturnsAsync(_project);

        var query = new GetTaskByIdQuery(userId, task.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(task.Status.ToString());
        result.Name.Should().Be(task.Name);
    }

    [Fact]
    public async Task Handle_Should_Throw_Forbbiden_When_Requester_Not_InPeople()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();
        var project = FakeProjectModelData.BuildProjectModel(Guid.NewGuid(),owner);
        var task = new TaskItem("task", Guid.Parse(project.Id), owner);


        _taskRepoMock.Setup(c=>c.GetByIdAsync(task.Id))
                .ReturnsAsync(task);
        _projectClientMock.Setup(c =>c.GetProjectAsync(project.Id))
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