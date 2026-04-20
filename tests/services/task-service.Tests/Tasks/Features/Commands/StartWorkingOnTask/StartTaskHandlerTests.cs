using task_service.Commons.Exceptions;
using task_service.Tasks.Features.Commands.StartWorkingOnTask;
using task_service.Tasks.Models;

namespace task_service.Tests.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<ILogger<StartTaskHandler>> _logger = new();

    private readonly StartTaskHandler _handler;

    public StartTaskHandlerTests()
    {
        _handler = new StartTaskHandler(
            _taskRepo.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Should_Start_Task_When_Assigned_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id))
                 .ReturnsAsync(task);

        var command = new StartTaskCommand(task.Id, userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        task.Status.Should().Be(task_service.Tasks.Models.TaskStatus.InProgress);
        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Assigned()
    {
        // Arrange
        var assignedUser = Guid.NewGuid();
        var otherUser = Guid.NewGuid();

        var task = new TaskItem("task", Guid.NewGuid(), assignedUser);
        task.AssignTo(assignedUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id))
                 .ReturnsAsync(task);

        var command = new StartTaskCommand(task.Id, otherUser);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, default));
    }

    [Fact]
    public async Task Should_Throw_When_Task_Not_Found()
    {
        // Arrange
        _taskRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync((TaskItem?)null);

        var command = new StartTaskCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, default));
    }

    [Fact]
    public async Task Should_Throw_When_Task_Already_Started()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId);
        task.StartWork();

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id))
                 .ReturnsAsync(task);

        var command = new StartTaskCommand(task.Id, userId);

        //Act 
        var action = () => _handler.Handle(command, CancellationToken.None);
        //  Assert
       await action.Should().ThrowAsync<TaskInvalidOperationException>().WithMessage("*already started*");
    }
}
