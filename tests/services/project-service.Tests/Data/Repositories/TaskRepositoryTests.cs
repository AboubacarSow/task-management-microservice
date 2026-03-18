using FluentAssertions;
using project_service.Tests.Fixtures;
using project_service.Tests.Helpers;
using System.Threading.Tasks;
using Task = project_service.Tasks.Models.Task;

namespace project_service.Tests.Data.Repositories;

[Collection("MongoDb")]
public class TaskRepositoryTests(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _databaseFixture = fixture;

    [Fact]
    public async System.Threading.Tasks.Task AddAsync_Then_GetById_ShouldReturnSameTaskAsync()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var task = new Task("Add Authentication", projectId, userId);

        await taskRepository.AddAsync(task);
        var new_added= await taskRepository.GetByIdAsync(task.Id);

        Assert.NotNull(new_added);
        new_added.Name.Should().Be(task.Name);
        new_added.CreatedByUser.Should().Be(userId);
        new_added.ProjectId.Should().Be(projectId);

    }
    [Fact]
    public async System.Threading.Tasks.Task GetAllByProjectId_ShouldReturnOnlyTaskForGivenProjectId()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var project1 = Guid.NewGuid();
        var project2 = Guid.NewGuid();

        var tasks = FakeTaskData.GetTasksForMultipleProjects(project1, project2);

        foreach (var task in tasks)
            await taskRepository.AddAsync(task);

        var result = await taskRepository.GetAllByProjectId(project1);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(4);
        result.Should().OnlyContain(t => t.ProjectId == project1);

    }
     [Fact]
    public async System.Threading.Tasks.Task GetAllByUserId_ShouldReturnOnlyTaskForGivenUserId()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var tasks = FakeTaskData.GetTasksForMultipleUsers(user1, user2);

        foreach (var task in tasks)
        {
            await taskRepository.AddAsync(task);

        }

        List<Task> result = await taskRepository.GetAllByUserIdAsync(user2);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(5);
        result.Should().OnlyContain(t => t.CreatedByUser == user2);

    }
}


