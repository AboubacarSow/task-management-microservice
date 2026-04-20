
using task_service.Tasks.Features.Queries.GetTasksByOwnerId;

namespace task_service.Tests.Tasks.Features.Queries.GetTasksByOwnerId;


public class GetTasksByOwnerIdHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ILogger<GetTasksByOwnerIdHandler>> _loggerMock = new();

    private readonly GetTasksByOwnerIdHandler _handler;

    public GetTasksByOwnerIdHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,_loggerMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Return_Tasks_When_Owner_Has_Tasks()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleUsers(ownerId,Guid.NewGuid());
        var ownerTasks = tasks.Where(t=>t.CreatedByUser == ownerId);

        _taskRepoMock.Setup(r => r.GetAllByOwnerIdAsync(ownerId))
            .ReturnsAsync([.. ownerTasks]);

        var query = new GetTasksByOwnerIdQuery(ownerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_Owner_Has_No_Tasks()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByOwnerIdAsync(ownerId))
            .ReturnsAsync([]);

        var query = new GetTasksByOwnerIdQuery(ownerId);

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
        var ownerId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var tasks = FakeTaskData.GetTasksForMultipleUsers(Guid.NewGuid(),ownerId);
        var ownerTasks = tasks.Where(t=>t.CreatedByUser == ownerId).ToList();

        _taskRepoMock.Setup(r => r.GetAllByOwnerIdAsync(ownerId))
            .ReturnsAsync(ownerTasks);


        var query = new GetTasksByOwnerIdQuery(ownerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var mappedProject = result.First();
        mappedProject.Id.Should().Be(ownerTasks[0].Id);
        mappedProject.Name.Should().Be(ownerTasks[0].Name);
        mappedProject.Description.Should().Be(ownerTasks[0].Description);
        mappedProject.CreatedAt.Should().Be(ownerTasks[0].CreatedAt);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_OnceAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByOwnerIdAsync(ownerId))
            .ReturnsAsync([]);

        var query = new GetTasksByOwnerIdQuery(ownerId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskRepoMock.Verify(r => r.GetAllByOwnerIdAsync(ownerId), Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Tasks_FoundAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByOwnerIdAsync(ownerId))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetTasksByOwnerIdQuery(ownerId), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(ownerId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

}
