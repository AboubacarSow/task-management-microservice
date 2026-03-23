using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using project_service.Data.Repositories;
using project_service.Projects.Features.Queries.GetPeopleWorkingById;
using project_service.Tests.Helpers;

namespace project_service.Tests.Projects.Features.Queries.GetPeopleWorkingById;


public class GetPeopleWorkingHandlerTests
{
    private readonly Mock<IProjectRepository> _repoMock = new();
    private readonly Mock<ILogger<GetPeopleWorkingByIdHandler>> _loggerMock = new();

    [Fact]
    public async Task Should_Return_PeopleWorking_When_User_Is_In_Group()
    {
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        var userId = Guid.NewGuid();
        project.AddUserToGroup(userId);

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(project);

        var handler = new GetPeopleWorkingByIdHandler(_repoMock.Object, _loggerMock.Object);

        List<Guid> result = await handler.Handle(new GetPeopleWorkingByIdQuery(userId, project.Id),CancellationToken.None);

        result.Should().Contain(userId);
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_In_Group()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());

        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(project);

        var handler = new GetPeopleWorkingByIdHandler(_repoMock.Object,_loggerMock.Object);

        Func<Task> act = async () => await handler.Handle(new GetPeopleWorkingByIdQuery(Guid.NewGuid(), project.Id),CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
