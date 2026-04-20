using project_service.Projects.Features.Commands.EditProject;

namespace project_service.Tests.Projects.Features.Commands.EditProject;

public class EditProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock = new();
    private readonly Mock<ILogger<EditProjectHandler>> _loggerMock = new();

    private readonly EditProjectHandler _handler;

    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _otherId = Guid.NewGuid();
    private readonly Guid _projectId = Guid.NewGuid();
    public EditProjectHandlerTests()
    {
        _handler = new(_repositoryMock.Object, _loggerMock.Object);
    }

    private EditProjectCommand BuildEditCommand(Guid? projectId = null,
    Guid? userId = null,
    string? description = null,
    DateTime? dueAt = null)=>
    new (projectId?? _projectId,
        userId ?? _ownerId,
        description,
        dueAt);

    
    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync((Project?)null);

        var command = BuildEditCommand();

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
         await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{_projectId}*");
    }

   
    [Fact]
    public async Task Handle_UserIsNotOwner_ThrowsForbiddenException()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(_ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildEditCommand(userId: _otherId);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"*{_otherId}*");
    }

    
    [Fact]
    public async Task Handle_ValidDescription_UpdatesDescriptionAndSaves()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(_ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildEditCommand(description: "Updated description");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Description.Should().Be("Updated description");
        _repositoryMock.Verify(r => r.EditAsync(project), Times.Once);
    }

   
    [Fact]
    public async Task Handle_ValidDueDate_UpdatesDueDateAndSaves()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(_ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var futureDate = DateTime.UtcNow.AddDays(7);
        var command    = BuildEditCommand(dueAt: futureDate);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.DueAt.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
        _repositoryMock.Verify(r => r.EditAsync(project), Times.Once);
    }

    
    [Fact]
    public async Task Handle_PastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(_ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildEditCommand(dueAt: DateTime.UtcNow.AddDays(-1));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
          await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*future*");
    }

    
    [Fact]
    public async Task Handle_NoFieldsProvided_SavesWithoutChanges()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(_ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_projectId))
            .ReturnsAsync(project);

        var command = BuildEditCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Description.Should().Be("Initial description");
        project.DueAt.Should().BeNull();
        _repositoryMock.Verify(r => r.EditAsync(project), Times.Once);
    }
}
