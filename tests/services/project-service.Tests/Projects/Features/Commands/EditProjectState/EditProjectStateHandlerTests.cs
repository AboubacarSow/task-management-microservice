using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Projects.Features.Commands.EditProjectState;
using project_service.Projects.Models;
using project_service.Tests.Helpers;

namespace project_service.Tests.Projects.Features.Commands.EditProjectState;

public class EditProjectStateHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ILogger<EditProjectStateHandler>> _loggerMock = new();

    private readonly EditProjectStateHandler _handler;

    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();
    private readonly Guid _projectId = Guid.NewGuid();

    public EditProjectStateHandlerTests()
    {
        _handler = new(
            _projectRepoMock.Object,
            _taskRepoMock.Object,
            _loggerMock.Object
        );
    }

    private EditProjectStateCommand BuildCommand(
        Guid? projectId = null,
        Guid? userId = null,
        ProjectStatus? status = null)
        => new(
            projectId ?? _projectId,
            userId ?? _ownerId,
            status ?? ProjectStatus.Active
        );

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync((Project?)null);

        var command = BuildCommand(status: ProjectStatus.Completed);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UserIsNotOwner_ThrowsForbiddenException()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildCommand(userId: _otherUserId, status: ProjectStatus.OnHold);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_Complete_WhenTasksNotCompleted_ThrowsDomainException()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        _taskRepoMock
            .Setup(t => t.AreAllTasksCompletedForProjectIdAsync(_projectId))
            .ReturnsAsync(false);

        var command = BuildCommand(status: ProjectStatus.Completed);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("*tasks*");
    }


    [Fact]
    public async Task Handle_Complete_WhenAllTasksCompleted_UpdatesState()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        _taskRepoMock
            .Setup(t => t.AreAllTasksCompletedForProjectIdAsync(_projectId))
            .ReturnsAsync(true);

        var command = BuildCommand(status: ProjectStatus.Completed);

        await _handler.Handle(command, CancellationToken.None);

        project.Status.Should().Be(ProjectStatus.Completed);
        _projectRepoMock.Verify(r => r.EditAsync(project), Times.Once);
    }


    [Fact]
    public async Task Handle_NonCompleteStatus_DoesNotCheckTasks()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildCommand(status: ProjectStatus.OnHold);

        await _handler.Handle(command, CancellationToken.None);

        _taskRepoMock.Verify(
            t => t.AreAllTasksCompletedForProjectIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }


    [Fact]
    public async Task Handle_PutOnHold_UpdatesState()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildCommand(status: ProjectStatus.OnHold);

        await _handler.Handle(command, CancellationToken.None);

        project.Status.Should().Be(ProjectStatus.OnHold);
        _projectRepoMock.Verify(r => r.EditAsync(project), Times.Once);
    }

    [Fact]
    public async Task Handle_Reactivate_UpdatesState()
    {
        var project = FakeProjectData.BuildProject(_ownerId);
        project.PutOnHold();

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildCommand(status: ProjectStatus.Active);

        await _handler.Handle(command, CancellationToken.None);

        project.Status.Should().Be(ProjectStatus.Active);
        _projectRepoMock.Verify(r => r.EditAsync(project), Times.Once);
    }

    [Fact]
    public async Task Handle_Archive_UpdatesState()
    {
        var project = FakeProjectData.BuildProject(_ownerId);

        _projectRepoMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildCommand(status: ProjectStatus.Archived);

        await _handler.Handle(command, CancellationToken.None);

        project.Status.Should().Be(ProjectStatus.Archived);
        _projectRepoMock.Verify(r => r.EditAsync(project), Times.Once);
    }
}