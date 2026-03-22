using FluentAssertions;
using Moq;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Projects.Features.Commands.AddUserToPeople;
using project_service.Projects.Models;
using project_service.Tests.Helpers;

namespace project_service.Tests.Projects.Features.Commands.AddUserToPeopleWorking;



public class AddUserToPeopleWorkingHandlerTests
{
    private readonly Mock<IProjectRepository> _repoMock = new();

    [Fact]
    public async Task Handle_Should_Add_User_To_PeopleWorking_And_Save()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId); 

        _repoMock.Setup(r => r.GetByIdAsync(projectId))
                 .ReturnsAsync(project);

        var handler = new AddUserToPeopleWorkingHandler(_repoMock.Object);

        var command = new AddUserToPeopleWorkingCommand(projectId, userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.PeopleWorking.Should().Contain(userId);

        _repoMock.Verify(r => r.EditAsync(project), Times.Once);
    }


    [Fact]
    public async Task Handle_Should_Throw_When_Project_Not_Found()
    {
        // Arrange

        
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync((Project?)null);

        var handler = new AddUserToPeopleWorkingHandler(_repoMock.Object);

        var command = new AddUserToPeopleWorkingCommand(projectId, userId);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }


    [Fact]
    public async Task Handle_Should_Throw_When_User_Not_In_Group()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(Guid.NewGuid());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(project);

        var handler = new AddUserToPeopleWorkingHandler(_repoMock.Object);

        var command = new AddUserToPeopleWorkingCommand(projectId, userId);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_Should_Not_Duplicate_User_In_PeopleWorking()
    {
        // Arrange
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);
        project.AddToPeopleWorking(userId); 

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(project);

        var handler = new AddUserToPeopleWorkingHandler(_repoMock.Object);

        var command = new AddUserToPeopleWorkingCommand(Guid.NewGuid(), userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.PeopleWorking.Count.Should().Be(1);
    }
}