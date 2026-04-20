
namespace task_service.Tests.Tasks.Features.Commands.CompleteTask;


public class CompleteTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<ILogger<CompleteTaskHandler>> _logger = new();

    private readonly CompleteTaskHandler _handler;

    public CompleteTaskHandlerTests()
    {
        _handler = new CompleteTaskHandler(
            _taskRepo.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Should_Complete_Task_When_Assigned_User()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId);
        // task need to be started in order to be marked as completed
        task.StartWork();

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var command = new CompleteTaskCommand(task.Id, userId);

        await _handler.Handle(command, CancellationToken.None);

        task.Status.Should().Be(task_service.Tasks.Models.TaskStatus.Completed);
        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_CurrentUser_Not_Assigned_User()
    {
        var assigned = Guid.NewGuid();
        var other = Guid.NewGuid();

        var task = new TaskItem("task", Guid.NewGuid(), assigned);
        task.AssignTo(assigned);
        task.StartWork();

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        // other user trying to marked task as completed, task not belonging to him
        var command = new CompleteTaskCommand(task.Id, other);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_When_Task_Not_InProgress()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId); 

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        // task with id : task.Id not in progress
        var command = new CompleteTaskCommand(task.Id, userId);

        await Assert.ThrowsAsync<TaskInvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}