using project_service.Projects.Features.Queries.GetProjectById;

namespace project_service.Tests.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock= new();
    private readonly Mock<ILogger<GetProjectByIdHandler>> _loggerMock= new();
    private readonly GetProjectByIdHandler _handler;

    public GetProjectByIdHandlerTests()
    {
        _handler = new GetProjectByIdHandler(_repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_ProjectDto_When_Project_ExistsAsync()
    {
        // Arrange
        var project = new Project("Building a web scraper", Guid.NewGuid());
        var projectId = project.Id;
     

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(project.OwnerId, projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(projectId);
        result.Name.Should().Be(project.Name);
        result.Description.Should().Be(project.Description);
    }

    [Fact]
    public void Handle_ShouldThrowNotFoundException_When_Project_Not_FoundAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(Guid.NewGuid(), projectId);

        // Act
        Action action= ()=> _handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult();

        // Assert
        action.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(Project)} with id: {projectId} not found");
    }

    [Fact]
    public void Handle_ShouldThrowForbiddenException_When_CurrentUser_Not_InPeopleWorking()
    {
        // Arrange
        var project = new Project("Building a web scraper", Guid.NewGuid());
        var projectId = project.Id;

        var currentUserId = Guid.NewGuid();  

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(currentUserId, projectId);

        // Act
        Action action = () => _handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult();

        // Assert
        action.Should().Throw<ForbiddenException>()
            .WithMessage($"User :{currentUserId} is not authorize to perform the [READ_PROJECT] operation");
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_OnceAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(Guid.NewGuid(), projectId);

        // Act
        _ = await Record.ExceptionAsync(() =>
        _handler.Handle(new GetProjectByIdQuery(Guid.NewGuid(), projectId), CancellationToken.None));

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_All_Fields_CorrectlyAsync()
    {
        // Arrange
        var project = new Project("Building a web scraper", Guid.NewGuid());
        project.SetDescription("Building using TDD approach");
        var projectId = project.Id;
        var userId = Guid.NewGuid();
        project.AddToPeopleWorking(userId);

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(userId, projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(project.Id);
        result.Name.Should().Be(project.Name);
        result.Description.Should().Be(project.Description);
        result.CreatedAt.Should().Be(project.CreatedAt);
    }

    [Fact]
    public async Task Handle_Should_Log_Information_When_Project_FoundAsync()
    {
        // Arrange
        var project = new Project("Building a web scraper", Guid.NewGuid());
        var projectId = project.Id;
        var userId = Guid.NewGuid();
        project.AddToPeopleWorking(userId);

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(new GetProjectByIdQuery(userId, projectId), CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(projectId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }


    [Fact]
    public async Task Handle_Should_Log_Warning_When_Project_Not_FoundAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        // Act
        _ = await Record.ExceptionAsync(() => 
        _handler.Handle(new GetProjectByIdQuery(Guid.NewGuid(), projectId), CancellationToken.None));

        // Assert
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) =>
                v.ToString()!.Contains(projectId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
