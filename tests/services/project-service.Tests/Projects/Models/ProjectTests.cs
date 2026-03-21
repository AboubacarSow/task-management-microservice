using FluentAssertions;
using project_service.Projects.Models;

namespace project_service.Tests.Projects.Models;
public class ProjectTests
{
    public readonly Guid UserId = Guid.NewGuid();
    private readonly string Name = "Yaz lab Project 1";
    private readonly string Description = @"Building a Task management Microservice with a gate way
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
        project.CreatedByUser.Should().Be(UserId);
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
 
  
}