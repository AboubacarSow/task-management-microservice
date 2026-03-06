using FluentAssertions;
using project_service.Tasks.Exceptions;
using Task = project_service.Tasks.Models.Task;
using TaskStatus = project_service.Tasks.Models.TaskStatus;


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

        Assert.NotEqual(Guid.Empty,task.Id);
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
    [Fact]
    public void AssignTo_ShouldStoreAssignedUserId()
    {
        var userToAssignId = Guid.NewGuid();
        _task.AssignTo(userToAssignId);

        Assert.NotEqual(_task.AssignedToUser, Guid.Empty);
        Assert.Equal(userToAssignId,_task.AssignedToUser);
    }
    [Fact]
    public void AssignTo_WithEmptyUserId_ShouldThrowException()
    {
        var empty_userId = Guid.Empty;

        var action = ()=> _task.AssignTo(empty_userId);

        action.Should().Throw<ArgumentException>()
            .WithMessage("User Id cannot be null or empty");
    }
    [Fact]
    public void AssignTo_ShouldUpdateLastUpdatedAt()
    {
        var previewsDate = _task.LastUpdatedAt;
        var before = DateTime.UtcNow;
        AssignTask();
        var after = DateTime.UtcNow;

        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);

    }

   
    [Fact]
    public void DefaultStatusOnCreation_ShouldBeToDo()
    {
        Assert.Equal(TaskStatus.ToDo,_task.Status);
    }

    [Fact]
    public void StartWork_ShouldChangeStatus_ToInProgress()
    {
        _task.StartWork();
        _task.Status.Should().Be(TaskStatus.InProgress);
    }
    [Fact]
    public void StartWork_ShouldUpdateLastUpdatedAt()
    {

        var previewsDate = _task.LastUpdatedAt;
        var before = DateTime.UtcNow;
        _task.StartWork();
        var after = DateTime.UtcNow;

        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);

    }
    [Fact]
    public void AfterSetDescription_ShouldNotBeNull()
    {
        var description = "Implementing Yarn to route all services";

        _task.SetDescription(description);

        _task.Description.Should().NotBe(null);
    }
    [Fact]
    public void SetDescription_WithEmptyDescription_ShouldThrowException()
    {
        var description = string.Empty;

        var action  = ()=> _task.SetDescription(description);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot be null or empty");
    }

    [Fact]
    public void Should_CompleteTask()
    {
        _task.CompleteTask();

        _task.Status.Should().Be(TaskStatus.Completed);

    }
    
    [Fact]
    public void UnAssign_ShouldSetAssignedUser_ToNull()
    {
        //Arrange
        AssignTask();
       
        _task.UnAssign();

        _task.AssignedToUser.Should().BeNull();
    }
    [Fact]
    public void UnAssigning_ShouldUpdateLastUpdatedAt()
    {
        //Arrange
        var previewsDate = _task.LastUpdatedAt;
        AssignTask();
        //Act
        var before = DateTime.UtcNow;
        _task.UnAssign();
        var after = DateTime.UtcNow;

        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);
    }
    [Fact]
    public void UnAssigning_OnCompletedTask_ShouldThrowDomainException()
    {
        _task.CompleteTask();

        var action  = _task.UnAssign;

        action.Should().Throw<TaskDomainException>()
            .WithMessage("Cannot unassign on completed task");

    }

    [Fact]
    public void UnAssigning_ShouldUpdateStatusTo_ToDoOrPause()
    {
        //Arrange
        AssignTask();
        _task.StartWork();
        //Act
        _task.UnAssign();
        _task.Status.Should().BeOneOf(TaskStatus.ToDo,TaskStatus.Pause);

    }

    [Fact]
    public void Should_ReassignToNewUser()
    {
        AssignTask();
        var new_userId= Guid.NewGuid();
        var previewsUserId= _task.AssignedToUser;
        //Act
        _task.ReassignTo(new_userId);

        //Assert
        Assert.NotEqual(new_userId,previewsUserId);
    }
    [Fact]
    public void Reassigning__ShouldUpdateLastUpdatedAt()
    {
        AssignTask();
        var new_userId= Guid.NewGuid();
        var previewsDate = _task.LastUpdatedAt;
        //Act
          var before = DateTime.UtcNow;
        _task.ReassignTo(new_userId);
        var after = DateTime.UtcNow;

        //Assert
        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);

    }

    [Fact]
    public void SetDueDate_WithValidDueDate_DueAt_ShouldNotBeNull()
    {
        
        var expectingDate = DateTime.Now.AddDays(2);

        //Act
        _task.SetDueAt(expectingDate);

        _task.DueAt.Should().Be(expectingDate);
    }

    [Fact]
    public void SetDueDate_WithInvalidDate_ShouldThrowException()
    {
        var expectingDate = DateTime.Now.AddDays(-1);

        //Act
        Action action = ()=> _task.SetDueAt(expectingDate);

        //Assert
        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("DueAt date must be in future.");
    }
    private void AssignTask()
    {
        var userToAssignId = Guid.NewGuid();
        _task.AssignTo(userToAssignId);
    }

}