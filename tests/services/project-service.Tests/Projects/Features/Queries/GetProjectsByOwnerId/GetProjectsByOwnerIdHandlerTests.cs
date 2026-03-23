using project_service.Projects.Features.Queries.GetProjectsByOwnerId;

namespace project_service.Tests.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock = new();
    private readonly Mock<ILogger<GetProjectsByOwnerIdHandler>> _loggerMock = new();
    private readonly GetProjectsByOwnerIdHandler _handler;

    public GetProjectsByOwnerIdHandlerTests()
    {
        _handler = new GetProjectsByOwnerIdHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Projects_When_Owner_Has_ProjectsAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var projects = FakeProjectData.GetProjectsForMultipleUsers(ownerId, ownerId);
        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync(projects);

        var query = new GetProjectsByOwnerIdQuery(ownerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_Owner_Has_No_ProjectsAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync([]);

        var query = new GetProjectsByOwnerIdQuery(ownerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Map_All_Fields_CorrectlyAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var projects = FakeProjectData.GetProjectsForMultipleUsers(ownerId, ownerId);

        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync(projects);

        var query = new GetProjectsByOwnerIdQuery(ownerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var mappedProject = result.First();
        mappedProject.Id.Should().Be(projects[0].Id);
        mappedProject.Name.Should().Be(projects[0].Name);
        mappedProject.Description.Should().Be(projects[0].Description);
        mappedProject.CreatedAt.Should().Be(projects[0].CreatedAt);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_OnceAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync([]);

        var query = new GetProjectsByOwnerIdQuery(ownerId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllByUserId(ownerId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Information_When_Projects_FoundAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var projects = FakeProjectData.GetProjectsForMultipleUsers(ownerId, ownerId);

        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new GetProjectsByOwnerIdQuery(ownerId), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(ownerId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Warning_When_No_Projects_FoundAsync()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetAllByUserId(ownerId))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(new GetProjectsByOwnerIdQuery(ownerId), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(ownerId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
