using project_service.Tasks.Features.Commands.CancelTask;
using project_service.Tasks.Features.Commands.PauseTask;
using TaskStatus = project_service.Tasks.Models.TaskStatus;

namespace project_service.Tests.Tasks.Features.Commands.PauseTask;

public class PauseTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly Mock<ILogger<PauseTaskHandler>> _logger = new();
    private readonly PauseTaskHandler _handler;

    public PauseTaskHandlerTests()
    {
        _handler = new PauseTaskHandler(_taskRepo.Object, _projectRepo.Object, _logger.Object);
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
        var project = new Project("test", owner);

        var task = new TaskItem("task", project.Id, owner);
        task.AssignTo(assignedUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new PauseTaskCommand(task.Id, outsider, "Break time");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}