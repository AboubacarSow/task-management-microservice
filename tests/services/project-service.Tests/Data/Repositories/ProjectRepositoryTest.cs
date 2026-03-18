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

    [Fact]
    public async Task GetAllByUserId_ShouldReturnOnlyProjectsForGivenUser()
    {
        //Arrange
        var projectRepository = FakeRepositories.GetProjectRepository
            (_databaseFixture.GetProjectCollection());

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var projects = FakeProjectData.GetProjectsForMultipleUsers(user1, user2);   

        foreach(var  project in projects)
            await projectRepository.AddAsync(project);

        //Act
        var result = await projectRepository.GetAllByUserId(user2);

        result.Should().NotBeNull();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p=>p.CreatedByUser == user2);
        
    }
}