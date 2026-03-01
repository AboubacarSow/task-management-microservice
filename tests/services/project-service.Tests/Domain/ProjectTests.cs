using FluentAssertions;
using project_service.Domain;

namespace project_service.Tests.Domain;
public class ProjectTests
{


    [Fact]
    public void CreateProject_WithValidNameAndDescription_ShouldCreateProject()
    {
        var name = "Yaz lab Project 1";
        var description = @"Building a Task management Microservice with a gate way
         that does not behave only as a proxy but also as a unit of work";

        var project = new Project(name, description);

        Assert.NotEmpty(project.Name);
        Assert.NotEmpty(project.Description);

        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
    }
    // [Fact]
    // public void CreateProject_WithEmpyNameAndDescription_ShouldThrowException()
    // {
    //     var name = string.Empty;
    //     var description =string.Empty;
         
    //     var project = new Project(name, description);

    //     Assert.NotEmpty(project.Name);
    //     Assert.NotEmpty(project.Description);
    // }
    [Fact]
    public void NewProject_ShouldHave_ActiveStatusByDefault()
    {
        
        var name = "Yaz lab Project 1";
        var description =@"Building a Task management Microservice with a gate way
         that does not behave only as a proxy but also as a unit of work";
        //Act
        var project = new Project(name, description);
        //Assert
        project.Status.Should().Be(ProjectStatus.Active);
    }

}