using Microsoft.Extensions.Logging;
using task_service.Commons.Exceptions;
using task_service.Data.Repositories;

namespace task_service.Tests.Tasks.Features.Commands.AssignTaskTo;

public class AssignTaskToHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<ILogger<AssignTaskToHandler>> _logger = new();

    private readonly AssignTaskToHandler _handler;

    public AssignTaskToHandlerTests()
    {
        _handler = new AssignTaskToHandler(
            _taskRepo.Object,
            _logger.Object);
    }

 

    [Fact]
    public async Task Should_Throw_When_Assigned_User_Not_In_Group()
    {
        //var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var currentUser = Guid.NewGuid();

        //var project = FakeProjectData.BuildProject(Guid.NewGuid());
        //project.AddUserToGroup(currentUser);

        var task = new TaskItem("task", project.Id, currentUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new AssignTaskToCommand(task.Id, Guid.NewGuid(), currentUser);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Assign_And_Add_To_PeopleWorking_When_Requester_In_Group()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var currentUser = Guid.NewGuid();
        var assignedUser = Guid.NewGuid();

        project.AddUserToGroup(currentUser);

        var task = new TaskItem("task", project.Id, assignedUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new AssignTaskToCommand(task.Id, currentUser, assignedUser);

        await _handler.Handle(command, CancellationToken.None);

        task.AssignedToUser.Should().Be(assignedUser);
        project.PeopleWorking.Should().Contain(assignedUser);

        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
        _projectRepo.Verify(x => x.EditAsync(project), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_Requester_Not_In_Group()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var currentUser = Guid.NewGuid();
        var assignedUser = Guid.NewGuid();

        var task = new TaskItem("task", project.Id, currentUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new AssignTaskToCommand(task.Id, assignedUser, currentUser);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
