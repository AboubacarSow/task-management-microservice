using Elasticsearch.Net.Specification.IndicesApi;
using Microsoft.Extensions.Logging;
using task_service.Commons.Exceptions;
using task_service.Data.Repositories;
using task_service.Tasks.Exceptions;
using task_service.Tasks.Features.Commands.CancelTask;
using task_service.Tasks.Grpc.Client;
using task_service.Tasks.Models;
using TaskStatus = task_service.Tasks.Models.TaskStatus;

namespace task_service.Tests.Tasks.Features.Commands.CancelTask;
public class CancelTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectClient> _projectClientMock = new();
    private readonly Mock<ILogger<CancelTaskHandler>> _loggerMock = new();
    private readonly CancelTaskHandler _handler;

    public CancelTaskHandlerTests()
    {
        _handler = new CancelTaskHandler(_taskRepo.Object, _projectClientMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CancelTask_ShouldThrow_WhenTaskAlreadyCompleted()
    {
        var userId = Guid.NewGuid();
        var projectOwner = Guid.NewGuid();
        var group = new List<Guid> { Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var project = FakeProjectModelData.BuildProjectModel(projectId,projectOwner,
            group.AsEnumerable());
         var task = new TaskItem("task", projectId, userId);
         task.AssignTo(userId);
        task.StartWork();
        task.CompleteTask();

         _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
         _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);
        var command = new CancelTaskCommand(userId, task.Id);

         await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
           .Should().ThrowAsync<TaskInvalidOperationException>();
    }

    [Fact]
    public async Task CancelTask_ShouldCallRepository_WhenValid()
    {
        var userId = Guid.NewGuid();
        var projectOwner = Guid.NewGuid();
        var group = new List<Guid> { Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var project = FakeProjectModelData.BuildProjectModel(projectId, projectOwner,
            group.AsEnumerable(),
            group.AsEnumerable());
        var task = new TaskItem("task", projectId, userId);
        task.AssignTo(userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);
        _taskRepo.Setup(x => x.EditAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

        var command = new CancelTaskCommand(userId, task.Id);
        await _handler.Handle(command, CancellationToken.None);

        _taskRepo.Verify(x => x.EditAsync(It.Is<TaskItem>(t => t.Status == TaskStatus.Cancelled)), Times.Once);
    }

      [Fact]
    public async Task Should_Throw_When_User_Not_In_Project()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();

        var group = new List<Guid> { Guid.NewGuid() };
        var projectId = Guid.NewGuid();
        var project = FakeProjectModelData.BuildProjectModel(projectId, Guid.NewGuid(),
            group.AsEnumerable(),
            group.AsEnumerable());
        var task = new TaskItem("task", projectId, owner);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectClientMock.Setup(x => x.GetProjectAsync(project.Id)).ReturnsAsync(project);

        var command = new CancelTaskCommand(outsider, task.Id);
         var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}