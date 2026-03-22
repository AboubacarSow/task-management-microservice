using FluentAssertions;
using project_service.Projects.Models;
using project_service.Tests.Helpers;

namespace project_service.Tests.Projects.Models;
public class ProjectTests
{
    public readonly Guid UserId = Guid.NewGuid();
    private readonly string Name = "Yaz lab Project 1";
    private readonly string Description = @"Building a TaskItem management Microservice with a gate way
         that does not behave only as a proxy but also as a unit of work";

    [Fact]
    public void CreateProject_WithValidName_ShouldCreateProject()
    {
        var project = new Project(Name,UserId);
        Assert.NotEmpty(project.Name);
        project.Name.Should().Be(Name);
    }

    [Fact]
    public void CreateProject_WithInvalidName_ShouldThrowException()
    {
        var name = string.Empty;

        Action action = () => new Project(name, UserId);

        action.Should().Throw<ArgumentException>()
        .WithMessage("Project name can not be empty");
    }
    [Fact]
    public void CreateProject_WithInvalidDescription_ShouldThrowException()
    {
        Action action = () => new Project(Name,UserId,"");
        action.Should().Throw<ArgumentException>().
        WithMessage("Project description can not be empty");
    }
    
    [Fact]
    public void CreateProject_WithValidNameAndDescription_ShouldCreateProject()
    {

        var project = new Project(Name,UserId, Description);

        //Assert
        Assert.NotEmpty(project.Name);
        Assert.NotEmpty(project.Description!);

        project.Name.Should().Be(Name);
        project.Description.Should().Be(Description);
    }

    
    [Fact]
    public void NewProject_ShouldHave_ActiveStatusByDefault()
    {
        //Act
        var project_v1 = new Project(Name,UserId);
        var project_v2 = new Project(Name,UserId, Description);
        //Assert
        project_v1.Status.Should().Be(ProjectStatus.Active);
        project_v2.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public void NewProject_ShouldHaveGeneratedId()
    {
       var project = new Project(Name,UserId);
       //Assert
       project.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CreateProject_ShouldSetCreatedAt()
    {
        var before = DateTime.UtcNow;
        
        var project = new Project(Name, UserId);

        var after = DateTime.UtcNow;

        project.CreatedAt.Should().BeOnOrAfter(before);
        project.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void LastUpdatedAtOnCreation_ShouldBeEqualToCreatedAt()
    {
        var project = new Project(Name, UserId);
        project.LastUpdatedAt.Should().Be(project.CreatedAt);
    }

    [Fact]
    public void SetDueDate_WithValidDueDate_DueAt_ShouldNotBeNull()
    {
        var project= new Project(Name, UserId);
        var expectingDate = DateTime.Now.AddDays(1);

        //Act
        project.SetDueDate(expectingDate);

        project.DueAt.Should().Be(expectingDate);
    }

    [Fact]
    public void SetDueDate_WithInvalidDate_ShouldThrowException()
    {
        var project = new Project(Name, UserId);
        var expectingDate = DateTime.Now.AddDays(-1);

        //Act
        Action action = ()=> project.SetDueDate(expectingDate);

        //Assert
        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("Due date must be in the future.");
    }

    [Fact]
    public void SetDueDate_ShouldUpdateLastUpdatedAt()
    {

        var project = new Project(Name, UserId);
        Task.Delay(TimeSpan.FromSeconds(8));
        var before = DateTime.UtcNow;
        var expectingDate = DateTime.Now.AddDays(8);
        project.SetDueDate(expectingDate);
        var after = DateTime.UtcNow;

        project.LastUpdatedAt.Should().NotBe(project.CreatedAt);
        project.LastUpdatedAt.Should().BeOnOrAfter(before);
        project.LastUpdatedAt.Should().BeOnOrBefore(after);


    }

    [Fact]
    public void CreateProject_ShouldStoreCreator()
    {
        var project = new Project(Name, UserId);
        project.OwnerId.Should().Be(UserId);
    }
    [Fact]
    public void CreateProject_WithEmptyCreatedBy_ShouldThrowException()
    {
        var action = ()=>new Project(Name,Guid.Empty);

        action.Should().Throw<ArgumentException>()
        .WithMessage("Project cannot be created without its [CreateByUser] specified");
    }
    [Fact]
    public void Should_SetDescription()
    {
        var project = new Project(Name,UserId);

        //Act
        project.SetDescription(Description);

        project.Description.Should().Be(Description);
    }

    [Fact]
    public void SetDescription_WithEmptyDescription_ShouldThrowException()
    {
        var project = new Project(Name,UserId);

        var action = ()=>project.SetDescription(string.Empty);

        action.Should().Throw<ArgumentException>()
        .WithMessage("Description cannot be empty or null");
    }



    //Status Tests

    [Fact]
    public void Complete_ActiveProject_SetsStatusToCompleted()
    {
        var project = BuildActiveProject();

        project.Complete();

        
        project.Status.Should().Be(ProjectStatus.Completed);
    }


    [Fact]
    public void Complete_OnHoldProject_ShouldThrowException()
    {
        var project = BuildOnHoldProject();

        var action = () =>project.Complete();

        action.Should().Throw<InvalidOperationException>().
        WithMessage("*OnHold project cannot be marked as Completed");
    }
    [Fact]
    public void Complete_ArchivedProject_ShouldThrowException()
    {
        var project = BuildArchivedProject();

        var action = () => project.Complete();

        action.Should().Throw<InvalidOperationException>().
        WithMessage("*archived");
    }

    [Fact]
    public void Complete_AlreadyCompletedProject_ThrowsInvalidOperationException()
    {
        
        var project = BuildCompletedProject();

        
        var act = () => project.Complete();

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already completed*");
    }

    [Fact]
    public void PutOnHold_ActiveProject_SetsStatusToOnHold()
    {
        
        var project = BuildActiveProject();

        project.PutOnHold();

        
        project.Status.Should().Be(ProjectStatus.OnHold);
    }
    [Fact]
    public void PutOnHold_CompletedProject_ThrowsInvalidOperationException()
    {
        var project = BuildCompletedProject();

        var act = () => project.PutOnHold();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*completed*");
    }
    [Fact]
    public void PutOnHold_ArchivedProject_ThrowsInvalidOperationException()
    {
        var project = BuildArchivedProject();

        var act = () => project.PutOnHold();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*archived*");
    }

    [Fact]
    public void PutOnHold_AlreadyOnHoldProject_ThrowsInvalidOperationException()
    {
        
        var project = BuildOnHoldProject();

        
        var act = () => project.PutOnHold();

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already on hold*");
    }

 
    [Fact]
    public void Reactivate_OnHoldProject_SetsStatusToActive()
    {
        
        var project = BuildOnHoldProject();

        
        project.Reactivate();

        project.Status.Should().Be(ProjectStatus.Active);
    }
    [Fact]
    public void Reactivate_ArchivedProject_ThrowsInvalidException()
    {

        var project = BuildArchivedProject();

        var action = ()=>project.Reactivate();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*archived*");
    }

    [Fact]
    public void Reactivate_ActiveProject_ThrowsInvalidOperationException()
    {
        var project = BuildActiveProject();
        
        var act = () => project.Reactivate();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already active*");
    }

    [Fact]
    public void Reactivate_CompletedProject_ThrowsInvalidOperationException()
    {
        var project = BuildCompletedProject();

        var act = () => project.Reactivate();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*completed*");
    }

    [Fact]
    public void Archive_ActiveProject_SetsStatusToArchived()
    {
        
        var project = BuildActiveProject();

        
        project.Archive();

        
        project.Status.Should().Be(ProjectStatus.Archived);
    }

    [Fact]
    public void Archive_OnHoldProject_SetsStatusToArchived()
    {
        
        var project = BuildOnHoldProject();

        
        project.Archive();

        
        project.Status.Should().Be(ProjectStatus.Archived);
    }

    [Fact]
    public void Archive_CompletedProject_SetsStatusToArchived()
    {
        
        var project = BuildCompletedProject();
        
        project.Archive();
        
        project.Status.Should().Be(ProjectStatus.Archived);
    }

    [Fact]
    public void Archive_AlreadyArchivedProject_ThrowsInvalidOperationException()
    {
        
        var project = BuildActiveProject();
        project.Archive();

        var act = () => project.Archive();
        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already archived*");
    }
    

    [Fact]
    public void AddUserToGroup_Should_Add_User()
    {
        var project = new Project("Test Project", Guid.NewGuid(), "Initial description");
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);

        project.Group.Should().Contain(userId);
    }

    [Fact]
    public void AddUserToGroup_Should_Not_Add_Duplicate_User()
    {
        var project = new Project("Test Project", Guid.NewGuid(), "Initial description");
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);
        project.AddUserToGroup(userId);

        project.Group.Count.Should().Be(1);
    }

    [Fact]
    public void AddUserToGroup_Should_Not_Add_Owner()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project("Test Project", ownerId, "Initial description");

        project.AddUserToGroup(ownerId);

        project.Group.Should().BeEmpty();
    }

    [Fact]
    public void AddUserToGroup_Should_Add_User_To_PeopleWorking()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);

        project.Group.Should().Contain(userId);
        project.PeopleWorking.Should().Contain(userId);
    }

    

    [Fact]
    public void IsInGroup_Should_Return_True_If_User_In_Group()
    {
        var project = new Project("Test Project", Guid.NewGuid(), "Initial description");
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);

        var result = project.IsInGroup(userId);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsInGroup_Should_Return_False_If_User_Not_In_Group()
    {
        var project = new Project("Test Project", Guid.NewGuid(), "Initial description");

        var result = project.IsInGroup(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public void Owner_Should_Always_Be_In_Group()
    {
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        bool result = project.IsInGroup(ownerId);

        result.Should().BeTrue();
    }

    [Fact]
    public void AddUserToGroup_Should_Not_Duplicate_In_PeopleWorking()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddUserToGroup(userId);
        project.AddUserToGroup(userId);

        project.PeopleWorking.Count.Should().Be(1+1);
    }
    [Fact]
    public void Owner_Should_Be_In_Group_And_PeopleWorking()
    {
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        project.IsInGroup(ownerId).Should().BeTrue();
        project.PeopleWorking.Should().Contain(ownerId);
    }
    [Fact]
    public void AddToPeopleWorking_Should_Add_User()
    {
        var project =   FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddToPeopleWorking(userId);

        project.PeopleWorking.Should().Contain(userId);
    }

    [Fact]
    public void AddToPeopleWorking_Should_Not_Add_Duplicate()
    {
        var project = FakeProjectData.BuildProject(Guid.NewGuid());
        var userId = Guid.NewGuid();

        project.AddToPeopleWorking(userId);
        project.AddToPeopleWorking(userId);

        project.PeopleWorking.Count.Should().Be(1+1);
    }

    
    [Fact]
    public void Owner_Should_Be_Allowed_In_PeopleWorking()
    {
        var ownerId = Guid.NewGuid();
        var project = FakeProjectData.BuildProject(ownerId);

        project.AddToPeopleWorking(ownerId);

        project.PeopleWorking.Should().Contain(ownerId);
    }

    private static Project BuildActiveProject() =>
        new ("Test Project", Guid.NewGuid(), "Initial description");

    private static Project BuildOnHoldProject()
    {
        var project = BuildActiveProject();
        project.PutOnHold();
        return project;
    }

    private static Project BuildCompletedProject()
    {
        var project = BuildActiveProject();
        project.Complete();
        return project;
    }

    private static Project BuildArchivedProject()
    {
        var project = BuildActiveProject();
        project.Archive();
        return project;
    }
 
  
}

