using task_service.Tasks.Features.Commands.EditTask;

namespace project_service.Tests.Tasks.Features.Commands.EditTask;

public class EditTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly Mock<ILogger<EditTaskHandler>> _loggerMock = new();
    private readonly EditTaskHandler _handler;

    public EditTaskHandlerTests()
    {
        _handler = new EditTaskHandler(_taskRepo.Object, 
            _projectRepo.Object,_loggerMock.Object);
    }

    [Fact]
    public async Task Should_Update_All_Fields()
    {
        var userId = Guid.NewGuid();
        var project = new Project("test", userId);

        var task = new TaskItem("old", project.Id, userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new EditTaskCommand(userId,
            task.Id,
            "new title",
            DateTime.UtcNow.AddDays(2),
            "new description"
        );

        await _handler.Handle(command, CancellationToken.None);

        task.Name.Should().Be("new title");
        task.Description.Should().Be("new description");

        _taskRepo.Verify(x => x.EditAsync(task), Times.Once);
    }

    [Fact]
    public async Task Should_Update_Without_Description_When_Null()
    {
        var userId = Guid.NewGuid();
        var project = new Project("test", userId);

        var task = new TaskItem("old", project.Id, userId);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);
        var dueAt = DateTime.UtcNow.AddDays(2);
        string des = "";
        var command = new EditTaskCommand(
            userId,
            task.Id,
            "updated",
            dueAt,
            des
        );

        await _handler.Handle(command, CancellationToken.None);

        task.Name.Should().Be("updated");
        

    }

    [Fact]
    public async Task Should_Throw_When_User_Not_In_Project()
    {
        var owner = Guid.NewGuid();
        var outsider = Guid.NewGuid();

        // In here we are not calling addUserToGroup
        var project = new Project("test", owner);
        
        var task = new TaskItem("task", project.Id, owner);

        _taskRepo.Setup(x => x.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _projectRepo.Setup(x => x.GetByIdAsync(project.Id)).ReturnsAsync(project);

        var command = new EditTaskCommand(
            outsider,
            task.Id,
            "new",
            DateTime.UtcNow.AddDays(3),
            null
        );

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}