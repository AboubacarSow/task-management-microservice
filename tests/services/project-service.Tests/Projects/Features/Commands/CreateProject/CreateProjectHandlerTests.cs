using project_service.Projects.Features.Commands.CreateProject;

namespace project_service.Tests.Projects.Features.Commands.CreateProject;


public class CreateProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock =new();
    private readonly Mock<ILogger<CreateProjectHandler>> _loggerMock=new();
    private readonly CreateProjectHandler _createCommandHandler;

    public CreateProjectHandlerTests()
    {
        _createCommandHandler = new CreateProjectHandler(_projectRepositoryMock.Object,
                                                        _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ForValidCommand_ReturnCreatedProjectId()
    {
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand("Building a web crawler in .Net",userId,null);


        _projectRepositoryMock.Setup(r=>r.AddAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);

        var result = await _createCommandHandler.Handle(command,CancellationToken.None);

        result.Should().NotBeEmpty();

        _projectRepositoryMock.Verify(r=>r.AddAsync(It.Is<Project>(p =>
                p.Name == command.Name &&
                p.OwnerId == command.CreatedByUser
            )),Times.Once());
            
    }
}
