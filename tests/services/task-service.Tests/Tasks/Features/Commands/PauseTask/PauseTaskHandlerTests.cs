using task_service.Commons.Exceptions;
using task_service.Tasks.Exceptions;
using task_service.Tasks.Features.Commands.PauseTask;
using task_service.Tasks.Grpc.Client;
using task_service.Tasks.Models;
using TaskStatus = task_service.Tasks.Models.TaskStatus;

namespace task_service.Tests.Tasks.Features.Commands.PauseTask;

public class PauseTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<ILogger<PauseTaskHandler>> _logger = new();
    private readonly PauseTaskHandler _handler;

    public PauseTaskHandlerTests()
    {
        _handler = new PauseTaskHandler(_taskRepo.Object,_logger.Object);
    }

    [Fact]
    public async Task PauseTask_ShouldThrow_WhenTaskNotInProgress()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        var command = new PauseTaskCommand(task.Id, userId, "Pause note");

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<TaskInvalidOperationException>();
    }

    [Fact]
    public async Task PauseTask_ShouldUpdateTaskStatus_WhenValid()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem("task", Guid.NewGuid(), userId);
        task.AssignTo(userId);
        task.StartWork();

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _taskRepo.Setup(x => x.EditAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

        var command = new PauseTaskCommand(task.Id, userId, "Break time");
        await _handler.Handle(command, CancellationToken.None);

        task.Status.Should().Be(TaskStatus.Pause);
        task.Note.Should().Be("Break time. ");
    }
    [Fact]
    public async Task Should_Throw_When_User_Not_AssignedUser()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();
        var assignedUser= Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var group = new List<Guid> { Guid.NewGuid() };
        var project = FakeProjectModelData.BuildProjectModel(projectId, Guid.NewGuid(),
            group.AsEnumerable());
        var task = new TaskItem("task", projectId, owner);
        task.AssignTo(assignedUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var command = new PauseTaskCommand(task.Id, outsider, "Break time");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}