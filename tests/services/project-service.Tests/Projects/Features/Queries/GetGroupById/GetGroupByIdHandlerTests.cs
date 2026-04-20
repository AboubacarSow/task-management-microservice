using project_service.Projects.Features.Queries.GetGroupById;

namespace project_service.Tests.Projects.Features.Queries.GetGroupById;


public class GetGroupByIHandlerTests
{
    private readonly Mock<IProjectRepository> _repoMock = new();
    private readonly Mock<ILogger<GetGroupByIdHandler>> _loggerMock = new();

    [Fact]
    public async Task Should_Return_Group_When_User_Is_Owner()
    {
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        var userId = Guid.NewGuid();
        project.AddUserToGroup(userId);

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(project);

        var handler = new GetGroupByIdHandler(_repoMock.Object,_loggerMock.Object);

        var  result = await handler.Handle(new GetGroupByIdQuery(ownerId,project.Id), CancellationToken.None);

        result.Should().Contain(userId);
    }

    [Fact]
    public async Task Should_Throw_When_User_Is_Not_Owner()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(project);

        var handler = new GetGroupByIdHandler(_repoMock.Object,_loggerMock.Object);

        Func<Task> act = async () => await handler.Handle(new GetGroupByIdQuery(Guid.NewGuid(),project.Id),CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
