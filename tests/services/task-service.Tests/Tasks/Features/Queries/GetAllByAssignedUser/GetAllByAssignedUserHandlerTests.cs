<<<<<<< HEAD


using project_service.Tasks.Features.Queries.GetAllByAssignedUser;

namespace project_service.Tests.Tasks.Features.Queries.GetAllByAssignedUser;


public class GetAllByAssignedUserHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ILogger<GetAllByAssignedUserHandler>> _loggerMock = new();

    private readonly GetAllByAssignedUserHandler _handler;

    public GetAllByAssignedUserHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,_loggerMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Return_Tasks_When_User_Has_Tasks()
    {
        // Arrange
        var assigned_user = Guid.NewGuid();
        var user1 = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleUsers(Guid.NewGuid(),user1);
        foreach(var task in tasks)
        {
            if (task.CreatedByUser == user1)
                task.AssignTo(assigned_user);
        }
        var assigned_tasks = tasks.Where(t=>t.AssignedToUser == assigned_user).ToList();
        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(assigned_user))
            .ReturnsAsync(assigned_tasks);

        var query = new GetAllByAssignedUserQuery(assigned_user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_User_Has_No_Tasks()
    {
        // Arrange
        var assignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(assignedUser))
            .ReturnsAsync([]);

        var query = new GetAllByAssignedUserQuery(assignedUser);

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
        var tasks = FakeTaskData.GetTasksForMultipleUsers(Guid.NewGuid(),AssignedUser);
        var ownerTasks = tasks.Where(t=>t.CreatedByUser == AssignedUser).ToList();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync(ownerTasks);


        var query = new GetAllByAssignedUserQuery(AssignedUser);

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
        var AssignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync([]);

        var query = new GetAllByAssignedUserQuery(AssignedUser);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskRepoMock.Verify(r => r.GetAllByAssignedUserIdAsync(AssignedUser), Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Tasks_FoundAsync()
    {
        // Arrange
        var AssignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetAllByAssignedUserQuery(AssignedUser), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(AssignedUser.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

=======

using task_service.Tasks.Features.Queries.GetAllByAssignedUser;

namespace task_service.Tests.Tasks.Features.Queries.GetAllByAssignedUser;


public class GetAllByAssignedUserHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ILogger<GetAllByAssignedUserHandler>> _loggerMock = new();

    private readonly GetAllByAssignedUserHandler _handler;

    public GetAllByAssignedUserHandlerTests()
    {
        _handler = new(_taskRepoMock.Object,_loggerMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Return_Tasks_When_User_Has_Tasks()
    {
        // Arrange
        var assigned_user = Guid.NewGuid();
        var user1 = Guid.NewGuid();
        var tasks = FakeTaskData.GetTasksForMultipleUsers(Guid.NewGuid(),user1);
        foreach(var task in tasks)
        {
            if (task.CreatedByUser == user1)
                task.AssignTo(assigned_user);
        }
        var assigned_tasks = tasks.Where(t=>t.AssignedToUser == assigned_user).ToList();
        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(assigned_user))
            .ReturnsAsync(assigned_tasks);

        var query = new GetAllByAssignedUserQuery(assigned_user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_User_Has_No_Tasks()
    {
        // Arrange
        var assignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(assignedUser))
            .ReturnsAsync([]);

        var query = new GetAllByAssignedUserQuery(assignedUser);

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
        var tasks = FakeTaskData.GetTasksForMultipleUsers(Guid.NewGuid(),AssignedUser);
        var ownerTasks = tasks.Where(t=>t.CreatedByUser == AssignedUser).ToList();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync(ownerTasks);


        var query = new GetAllByAssignedUserQuery(AssignedUser);

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
        var AssignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync([]);

        var query = new GetAllByAssignedUserQuery(AssignedUser);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskRepoMock.Verify(r => r.GetAllByAssignedUserIdAsync(AssignedUser), Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Tasks_FoundAsync()
    {
        // Arrange
        var AssignedUser = Guid.NewGuid();

        _taskRepoMock.Setup(r => r.GetAllByAssignedUserIdAsync(AssignedUser))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetAllByAssignedUserQuery(AssignedUser), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(AssignedUser.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

>>>>>>> 05b451b (new_update)
}