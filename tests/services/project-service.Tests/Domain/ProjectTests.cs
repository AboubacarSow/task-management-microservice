using FluentAssertions;
using project_service.Domain;

namespace project_service.Tests.Domain;
public class ProjectTests
{
   [Fact]
   public void CreateProject_WithValidName_ShouldCreateProject()
    {
        //Arrange
        var name = "Yaz lab Project 1";

        //Act
        var project = new Project(name);

        //Assert
        project.Name.Should().Be(name);
        
    } 

    [Fact]
    public void CreateProject_WithEmptyName_ShouldThrowException()
    {
        //Arrang
        var name=string.Empty;

        //Act
        Action action  = ()=> new Project(name);

        //Assert
       action.Should().Throw<ArgumentException>()
       .WithMessage("Project name can not be empty");
    }
}