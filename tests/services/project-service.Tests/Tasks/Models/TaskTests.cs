using FluentAssertions;
using Task = project_service.Tasks.Models.Task;
namespace project_service.Tests.Tasks.Models;

public class TaskTests
{

    private readonly string _name = "Task Test";
    private readonly Guid _projectId=Guid.NewGuid();
    private readonly Guid _emptyProjectId = Guid.Empty;

    private readonly Task _task = new("Old Test",Guid.NewGuid());
    [Fact]
    public void CreateTask_WithValidName_ShouldCreateTask()
    {
        var task = new Task(_name,_projectId);

        task.Should().NotBeNull();
        task.Name.Should().Be(_name);
    }

    [Fact]
    public void CreateProject_WithEmptyName_ShouldThrowException()
    {
        var emptyName = "";
        var createAction = ()=>new Task(emptyName,_projectId);

        createAction.Should().Throw<ArgumentException>()
        .WithMessage("Task name cannot be null or empty");

    }

    [Fact]
    public void CreateTask_WithoutProjectId_ShouldThrowException()
    {

        var createAction =()=> new Task(_name,_emptyProjectId);

        createAction.Should().Throw<ArgumentException>()
        .WithMessage("Task cannot be created without its project specified");

        
    }

    [Fact]
    public void CreateTask_OnAfter_ProjectId_ShouldNotBeNull()
    {
        var task = new Task(_name,_projectId);

        Assert.NotEqual(task.ProjectId, _emptyProjectId);
        task.ProjectId.Should().Be(_projectId);
    }

    [Fact]
    public void NewTask_ShouldHaveGeneratedId()
    {
        var task = new Task(_name,_projectId);

        Assert.NotEqual(task.Id, Guid.Empty);
    }

    [Fact]
    public void CreateTask_ShouldStoreCreatedAtDate()
    {
        var before = DateTime.UtcNow;
        var task = new Task(_name,_projectId);
        var after = DateTime.UtcNow;

        task.CreatedAt.Should().BeOnOrAfter(before);
        task.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact] 
    public void LastUpdatedAtOnCreation_ShouldBeEqualToCreatedAt()
    {
        var task = new Task(_name,_projectId);

        task.LastUpdatedAt.Should().Be(task.CreatedAt);
    }


}