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

        Action action  = () => new Project(name,UserId);

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
        project.CreatedBy.Should().Be(UserId);
    }

 
}