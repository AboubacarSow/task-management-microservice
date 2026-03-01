using FluentAssertions;
using project_service.Domain;

namespace project_service.Tests.Domain;
public class ProjectTests
{

    [Fact]
    public void CreateProject_WithValidName_ShouldCreateProject(){
        var name = "Yaz lab Project 1";
        var project = new Project(name);
        Assert.NotEmpty(project.Name);
        project.Name.Should().Be(name);
    }

    [Fact]
    public void CreateProject_WithInvalidName_ShouldThrowException(){
        var name = string.Empty;

        Action action  = () => new Project(name);

        action.Should().Throw<ArgumentException>()
        .WithMessage("Project name can not be empty");
    }
    [Fact]
    public void CreateProject_WithInvalidDescription_ShouldThrowException(){
        var name = "Yaz lab Project 1";

        Action action = () => new Project(name,"");
        action.Should().Throw<ArgumentException>().
        WithMessage("Project description can not be empty");
    }
            


    [Fact] 
    public void CreateProject_WithEmptyDescription_ShouldNotSetDescription()
    {
        var name = "Yaz lab Project 1";
        var project = new Project(name);

        Assert.Null(project.Description);
    }
    
    [Fact]
    public void CreateProject_WithValidNameAndDescription_ShouldCreateProject()
    {
        var name = "Yaz lab Project 1";
        var description = @"Building a Task management Microservice with a gate way
         that does not behave only as a proxy but also as a unit of work";

        var project = new Project(name, description);

        Assert.NotEmpty(project.Name);
        Assert.NotEmpty(project.Description!);

        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
    }

    
    [Fact]
    public void NewProject_ShouldHave_ActiveStatusByDefault()
    {
        
        var name = "Yaz lab Project 1";
        var description =@"Building a Task management Microservice with a gate way
         that does not behave only as a proxy but also as a unit of work";
        //Act
        var project_v1 = new Project(name);
        var project_v2 = new Project(name, description);
        //Assert
        project_v1.Status.Should().Be(ProjectStatus.Active);
        project_v2.Status.Should().Be(ProjectStatus.Active);
    }

}