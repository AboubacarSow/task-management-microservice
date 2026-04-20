<<<<<<< HEAD
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
=======
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.messaging.Events;
using task_service.Commons.Exceptions;
using task_service.Data.Repositories;
using task_service.Tasks.Grpc.Client;

namespace task_service.Tests.Tasks.Features.Commands.AssignTaskTo;

public class AssignTaskToHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectClient>  _projectClientMock = new();
    private readonly Mock<IPublishEndpoint> _publisherMock = new();
    private readonly Mock<ILogger<AssignTaskToHandler>> _logger = new();

    private readonly AssignTaskToHandler _handler;

    public AssignTaskToHandlerTests()
    {
        _handler = new AssignTaskToHandler(
            _taskRepo.Object,_projectClientMock.Object,
            _publisherMock.Object,
            _logger.Object);
    }

 

   
    [Fact]
    public async Task Should_Assign_And_Add_To_PeopleWorking_When_Requester_In_Group()
    {
        var currentUser = Guid.NewGuid();
        var projectId = Guid.NewGuid(); 
        var group = new List<Guid> { currentUser };
        var project = FakeProjectModelData.BuildProjectModel(projectId,Guid.NewGuid(),
            group.AsEnumerable());
        var assignedUser = Guid.NewGuid();


        var task = new TaskItem("task", projectId, assignedUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        project.PeopleWorking.Add(assignedUser.ToString());
        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);


        var command = new AssignTaskToCommand(task.Id, currentUser, assignedUser);

        await _handler.Handle(command, CancellationToken.None);

        task.AssignedToUser.Should().Be(assignedUser);
        project.PeopleWorking.Should().Contain(assignedUser.ToString());

        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
        _publisherMock.Verify(x => x.Publish(
                    It.Is<TaskAssignedIntegrationEvent>(e =>
                        e.TaskId == task.Id &&
                        e.ProjectId == task.ProjectId &&
                        e.AssignedUserId == assignedUser
                    ),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_Requester_Not_In_Group()
    {
        var group = new List<Guid> { Guid.NewGuid() };
        var project = FakeProjectModelData.BuildProjectModel(Guid.NewGuid(),Guid.NewGuid(),
            group.AsEnumerable());
        var currentUser = Guid.NewGuid();
        var projectId = Guid.Parse(project.Id);
        var assignedUser = Guid.NewGuid();

        var task = new TaskItem("task", projectId, currentUser);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);

        var command = new AssignTaskToCommand(task.Id, assignedUser, currentUser);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
>>>>>>> 05b451b (new_update)
