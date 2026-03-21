using FluentAssertions;
using project_service.Tasks.Exceptions;
using project_service.Tasks.Models;
using TaskStatus = project_service.Tasks.Models.TaskStatus;


namespace project_service.Tests.Tasks.Models;

public class TaskTests
{

    private readonly string _name = "TaskItem Test";
    private readonly Guid _projectId=Guid.NewGuid();
    private readonly Guid _userId=Guid.NewGuid();
    private readonly Guid _emptyProjectId = Guid.Empty;
    private readonly TaskItem _task = new("Old Test",Guid.NewGuid(), Guid.NewGuid());
    [Fact]
    public void CreateTask_WithValidName_ShouldCreateTask()
    {
        var task = new TaskItem(_name,_projectId, _userId);

        task.Should().NotBeNull();
        task.Name.Should().Be(_name);
    }
    [Fact]
    public void CreateTask_ShouldStoreCreatedByUser()
    {
        var task = new TaskItem(_name,_projectId, _userId);

        task.CreatedByUser.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CreateTask_OnAfter_CreatedByUser_ShouldNotBeNull()
    {
        var task = new TaskItem(_name,_projectId, _userId);

        Assert.NotEqual(task.CreatedByUser, Guid.Empty);
        task.CreatedByUser.Should().Be(_userId);
    }
    [Fact]
    public void CreateTask_WithoutCreatedByUser_ShouldThrowException()
    {

        var createAction = () => new TaskItem(_name, _projectId, Guid.Empty);

        createAction.Should().Throw<ArgumentException>()
        .WithMessage("TaskItem cannot be created without its user author specified");


    }
    [Fact]
    public void CreateProject_WithEmptyName_ShouldThrowException()
    {
        var emptyName = "";
        var createAction = ()=>new TaskItem(emptyName,_projectId, _userId);

        createAction.Should().Throw<ArgumentException>()
        .WithMessage("TaskItem name cannot be null or empty");

    }

    [Fact]
    public void CreateTask_WithoutProjectId_ShouldThrowException()
    {

        var createAction =()=> new TaskItem(_name,_emptyProjectId, _userId);

        createAction.Should().Throw<ArgumentException>()
        .WithMessage("TaskItem cannot be created without its project specified");

        
    }

    [Fact]
    public void CreateTask_OnAfter_ProjectId_ShouldNotBeNull()
    {
        var task = new TaskItem(_name,_projectId, _userId);

        Assert.NotEqual(task.ProjectId, _emptyProjectId);
        task.ProjectId.Should().Be(_projectId);
    }

    [Fact]
    public void NewTask_ShouldHaveGeneratedId()
    {
        var task = new TaskItem(_name,_projectId, _userId);

        Assert.NotEqual(Guid.Empty,task.Id);
    }

    [Fact]
    public void CreateTask_ShouldStoreCreatedAtDate()
    {
        var before = DateTime.UtcNow;
        var task = new TaskItem(_name,_projectId, _userId);
        var after = DateTime.UtcNow;

        task.CreatedAt.Should().BeOnOrAfter(before);
        task.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact] 
    public void LastUpdatedAtOnCreation_ShouldBeEqualToCreatedAt()
    {
        var task = new TaskItem(_name,_projectId, _userId);

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
    public void DefaultPriorityOnCreation_ShouldBeMedium()
    {
        Assert.Equal(TaskPriority.Medium,_task.Priority);
    }

    [Fact]
    public void StartWork_ShouldChangeStatus_ToInProgress()
    {
        //Arrane
        AssignTask();
        //Act
        _task.StartWork();

        _task.Status.Should().Be(TaskStatus.InProgress);
    }
    [Fact]
    public void StartWork_ShouldUpdateLastUpdatedAt()
    {
        //Arrange
        AssignTask();

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
    public void StartWork_WhenNot_NotAssigned_ShouldThrowException()
    {
         var action = _task.StartWork;
         action.Should().Throw<TaskInvalidOperationException>()
         .WithMessage("TaskItem must be assigned first to be started");
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
    public void CompleteTask_ShouldChangeStatus_ToCompleted()
    {
        //Arrange
        AssignTask();
        _task.StartWork();
        var note= "Finished successfully";

        //Act
        _task.CompleteTask(note);

        _task.Status.Should().Be(TaskStatus.Completed);
        Assert.Equal(note, _task.Note);

    }
    [Fact]
    public void Should_CompleteTask_UpdateLastUpdatedAt()
    {
         AssignTask();
        _task.StartWork();
         var previewsDate = _task.LastUpdatedAt;
        //Act
          var before = DateTime.UtcNow;
        _task.CompleteTask();
        var after = DateTime.UtcNow;

        //Assert
        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);

    }
    [Fact]
    public void CompleteTask_WhenNot_InProgress_ShouldThrowException()
    {
        var action = ()=> _task.CompleteTask();

        action.Should().Throw<TaskInvalidOperationException>()
        .WithMessage("Cannot mark as completed a task not in Progress");
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
        CompleteTaskTest();

        var action  = _task.UnAssign;

        action.Should().Throw<TaskInvalidOperationException>()
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
    
    [Fact]

    public void Cancel_ShouldChangeStatusTo_Cancelled()
    {
        _task.Cancel();

        Assert.Equal(TaskStatus.Cancelled,_task.Status);
    }
    [Fact]
    public void Cancel_OnCompletedTask_ShouldThrowException()
    {
        //Arrange
        CompleteTaskTest();

        //Act
        var action = _task.Cancel;

        action.Should().Throw<TaskInvalidOperationException>()
            .WithMessage("Cannot cancel a completed TaskItem");

    }

    [Fact]
    public void Cancel_ShouldUpdate_LastUpdatedAt()
    {
         var previewsDate = _task.LastUpdatedAt;
        //Act
          var before = DateTime.UtcNow;
        _task.Cancel();
        var after = DateTime.UtcNow;

        //Assert
        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);
    }
    [Fact]
    public void Block_ShouldChangeStatus_ToPause(){
        //Arrrage
        AssignTask();
        _task.StartWork();
        _task.Block("Due to some reason");

        Assert.Equal(TaskStatus.Pause,_task.Status);
    }
    [Fact]
    public void Block_WhenNot_InProgress_ShouldThrowException()
    {
     
        var action  = ()=>_task.Block("Due to some reason");

        action.Should().Throw<TaskInvalidOperationException>()
            .WithMessage("Cannot perform this operation.TaskItem is not in progress");
    }

    [Fact]
    public void Block_WithEmptyNote_ShouldThrowException()
    {
         AssignTask();
        _task.StartWork();
        var action = ()=>_task.Block("");

        action.Should().Throw<ArgumentException>().
        WithMessage("While Blocking task, note message cannot be null or empty");
    }

    [Fact]
    public void Block_Should_UpdatedLastUpdatedAt(){
        AssignTask();
        _task.StartWork();
         var previewsDate = _task.LastUpdatedAt;
        //Act
          var before = DateTime.UtcNow;
        _task.Block("For some reason, I paused this task");
        var after = DateTime.UtcNow;

        //Assert
        _task.LastUpdatedAt.Should().BeOnOrAfter(before);
        _task.LastUpdatedAt.Should().BeOnOrBefore(after);
        _task.LastUpdatedAt.Should().NotBe(previewsDate);
        Assert.True(_task.LastUpdatedAt > previewsDate);
    }


    private void AssignTask()
    {
        var userToAssignId = Guid.NewGuid();
        _task.AssignTo(userToAssignId);
    }

    private void CompleteTaskTest()
    {

        AssignTask();
        _task.StartWork();
        _task.CompleteTask();
    }

}