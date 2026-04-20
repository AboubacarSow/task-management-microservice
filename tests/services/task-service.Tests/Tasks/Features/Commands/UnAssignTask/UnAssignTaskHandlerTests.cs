using task_service.Commons.Exceptions;
using task_service.Tasks.Exceptions;
using task_service.Tasks.Features.Commands.UnAssignTask;

namespace task_service.Tests.Tasks.Features.Commands.UnAssignTask;

public class UnassignTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<ILogger<UnAssignTaskHandler>> _loggerMock = new();
    private readonly UnAssignTaskHandler _handler;
    private readonly Guid ownerId = Guid.NewGuid();
    private readonly Guid projectId = Guid.NewGuid();
    public UnassignTaskHandlerTests()
    {
        _handler = new UnAssignTaskHandler(_taskRepo.Object,_loggerMock.Object);
    }

    [Fact]
    public async Task Should_Unassign_When_User_Is_AssignedUser()
    {
      
        var task = new TaskItem("task", projectId, ownerId);

        var other_user= Guid.NewGuid();
        task.AssignTo(other_user);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        var command = new UnAssignTaskCommand(task.Id,ownerId);

        await _handler.Handle(command, CancellationToken.None);

        task.AssignedToUser.Should().BeNull();
        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_User_Is_NotTaskOwner()
    {
        var other = Guid.NewGuid();

        var task = new TaskItem("task", projectId, ownerId);
        task.AssignTo(other);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var command = new UnAssignTaskCommand(task.Id, Guid.NewGuid());

        var action = () =>_handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenException>();
    }


    [Fact]
    public async Task Should_Throw_When_Task_Not_Assigned()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var task = new TaskItem("task", projectId, userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var command = new UnAssignTaskCommand(task.Id, userId);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<TaskInvalidOperationException>();
    }
}
