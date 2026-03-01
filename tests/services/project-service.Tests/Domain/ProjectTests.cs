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
}