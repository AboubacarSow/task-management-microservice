<<<<<<< HEAD
using project_service.Projects.Features.Commands.AddUserToGroup;

namespace project_service.Tests.Projects.Features.Commands.AddUserToGroup;

public class AddUserToGroupHandlerTests
{
    private readonly Mock<IProjectRepository> _repoMock = new();
    private readonly Mock<ILogger<AddUserToGroupHandler>> _loggerMock = new();


    [Fact]
    public async Task Handle_Should_Add_User_To_Group_And_Save()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(projectId))
                 .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, projectId,ownerId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Group.Should().Contain(userId);
        project.PeopleWorking.Should().Contain(userId);

        _repoMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _repoMock.Verify(r => r.EditAsync(project), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Duplicate_User_In_Group()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, Guid.NewGuid(), Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Group.Count.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Throw_ForbiddenException_When_User_Is_Not_Project_Owner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }
}


=======
using project_service.Projects.Features.Commands.AddUserToGroup;

namespace project_service.Tests.Projects.Features.Commands.AddUserToGroup;

public class AddUserToGroupHandlerTests
{
    private readonly Mock<IProjectRepository> _repoMock = new();
    private readonly Mock<ILogger<AddUserToGroupHandler>> _loggerMock = new();


    [Fact]
    public async Task Handle_Should_Add_User_To_Group_And_Save()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(projectId))
                 .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, projectId,ownerId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Group.Should().Contain(userId);
        project.PeopleWorking.Should().Contain(userId);

        _repoMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _repoMock.Verify(r => r.EditAsync(project), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Duplicate_User_In_Group()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, Guid.NewGuid(), ownerId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Group.Count.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Throw_ForbiddenException_When_User_Is_Not_Project_Owner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

        var handler = new AddUserToGroupHandler(_repoMock.Object,_loggerMock.Object);

        var command = new AddUserToGroupCommand(userId, Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }
}


>>>>>>> 05b451b (new_update)
