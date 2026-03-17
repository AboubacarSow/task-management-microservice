using FluentAssertions;
using project_service.Projects.Models;
using project_service.Tests.Fixtures;
using project_service.Tests.Helpers;
using System.Threading.Tasks;

namespace project_service.Tests.Data.Repositories;

[Collection("MongoDb")]
public class ProjectRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _databaseFixture = fixture;

    [Fact]
    public async Task AddAsync_Then_GetById_ShouldReturnSameProject()
    {
        //Arrange
        var userId= Guid.NewGuid(); 
        var project = new Project("Software Development", userId);
        var projectRepository = FakeRepositories.GetProjectRepository
            (_databaseFixture.GetProjectCollection());

        //Act
        await projectRepository.AddAsync(project);
        Project result = await projectRepository.GetByIdAsync(project.Id);


        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Name.Should().Be(project.Name);
        result.CreatedByUser.Should().Be(project.CreatedByUser);
        
    }
}