using project_service.Tasks.Features.Commands.UnAssignTask;

namespace project_service.Tests.Tasks.Features.Commands.UnAssignTask;

public class UnassignTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly UnAssignTaskHandler _handler;

    public UnassignTaskHandlerTests()
    {
        _handler = new UnAssignTaskHandler(_taskRepo.Object, _projectRepo.Object);
    }

    [Fact]
    public async Task Should_Unassign_When_User_Is_AssignedUser()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var project = new Project("test", userId);
        var task = new TaskItem("task", project.Id, userId);

        var other_user= Guid.NewGuid();
        task.AssignTo(other_user);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UnAssignTaskCommand(task.Id, userId);

        await _handler.Handle(command, CancellationToken.None);

        task.AssignedToUser.Should().BeNull();
        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_User_Is_NotTaskOwner()
    {
        var owner = Guid.NewGuid();
        var other = Guid.NewGuid();

        var project = new Project("test", Guid.NewGuid());
        project.AddUserToGroup(other);

        var task = new TaskItem("task", project.Id, owner);
        task.AssignTo(other);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UnAssignTaskCommand(task.Id, Guid.NewGuid());

        var action = () =>_handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Should_Throw_When_Not_Authorized()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();

        var project = new Project("test", owner);
        var task = new TaskItem("task", project.Id, owner);
        task.AssignTo(owner);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UnAssignTaskCommand(task.Id, outsider);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Should_Throw_When_Task_Not_Assigned()
    {
        var userId = Guid.NewGuid();
        var project = new Project("test", userId);
        //project.AddToGroup(userId);

        var task = new TaskItem("task", project.Id, userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new UnAssignTaskCommand(task.Id, userId);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<TaskInvalidOperationException>();
    }
}