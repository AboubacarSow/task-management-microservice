using FluentAssertions;
using project_service.Tests.Fixtures;
using project_service.Tests.Helpers;
using project_service.Tasks.Models;

namespace project_service.Tests.Data.Repositories;

[Collection("MongoDb")]
public class TaskRepositoryTests(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _databaseFixture = fixture;

    [Fact]
    public async Task AddAsync_Then_GetById_ShouldReturnSameTaskAsync()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var task = new TaskItem("Add Authentication", projectId, userId);

        await taskRepository.AddAsync(task);
        var new_added= await taskRepository.GetByIdAsync(task.Id);

        Assert.NotNull(new_added);
        new_added.Name.Should().Be(task.Name);
        new_added.CreatedByUser.Should().Be(userId);
        new_added.ProjectId.Should().Be(projectId);

    }
    [Fact]
    public async Task GetAllByProjectId_ShouldReturnOnlyTaskForGivenProjectId()
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
    public async Task GetAllByUserId_ShouldReturnOnlyTaskForGivenUserId()
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

        List<TaskItem> result = await taskRepository.GetAllByUserIdAsync(user2);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(5);
        result.Should().OnlyContain(t => t.CreatedByUser == user2);

    }


    [Fact]
    public async Task EditAsync_ShouldUpdateTaskFields()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var task = new TaskItem("Software Development", Guid.NewGuid(),Guid.NewGuid());
        await taskRepository.AddAsync(task);

        var oldTask = await taskRepository.GetByIdAsync(task.Id)!;
        oldTask.SetDescription("This task is about implement the gateway using ocelot");
        oldTask.SetDueAt(DateTime.UtcNow.AddDays(6));
        await taskRepository.EditAsync(oldTask);

        var editedTask = await taskRepository.GetByIdAsync(task.Id);

        editedTask.Should().NotBeNull();
        editedTask.Id.Should().Be(task.Id);
        editedTask.CreatedAt.Should().BeCloseTo(task.CreatedAt, TimeSpan.FromMilliseconds(1));
        editedTask.Description.Should().Be(oldTask.Description);
        editedTask.DueAt.Should().BeCloseTo((DateTime)oldTask.DueAt!, TimeSpan.FromMilliseconds(1));
        editedTask.LastUpdatedAt.Should().BeCloseTo((DateTime)oldTask.LastUpdatedAt!, TimeSpan.FromMilliseconds(1));

    }

    [Fact]
    public async Task AreAllTasksCompleted_NoTasks_ReturnsTrue()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();

        bool result = await taskRepository
            .AreAllTasksCompletedForProjectIdAsync(projectId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AreAllTasksCompleted_AllTasksCompleted_ReturnsTrue()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new ("Task 1", projectId, userId),
            new ("Task 2", projectId, userId)
        };

        foreach (var task in tasks)
        {
            CompleteTaskTest(task); 
            await taskRepository.AddAsync(task);
        }

        var result = await taskRepository
            .AreAllTasksCompletedForProjectIdAsync(projectId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AreAllTasksCompleted_OneIncomplete_ReturnsFalse()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var completedTask = new TaskItem("Done Task", projectId, userId);
        CompleteTaskTest(completedTask);

        var incompleteTask = new TaskItem("Pending Task", projectId, userId);

        await taskRepository.AddAsync(completedTask);
        await taskRepository.AddAsync(incompleteTask);

        var result = await taskRepository
            .AreAllTasksCompletedForProjectIdAsync(projectId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AreAllTasksCompleted_AllIncomplete_ReturnsFalse()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new ("Task 1", projectId, userId),
            new ("Task 2", projectId, userId)
        };

        foreach (var task in tasks)
            await taskRepository.AddAsync(task);

        var result = await taskRepository
            .AreAllTasksCompletedForProjectIdAsync(projectId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AreAllTasksCompleted_ShouldIgnoreOtherProjects()
    {
        var taskRepository = FakeRepositories.GetTaskRepository
            (_databaseFixture.GetTaskCollection());

        var targetProject = Guid.NewGuid();
        var otherProject = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var validTask = new TaskItem("Valid Task", targetProject, userId);
        CompleteTaskTest(validTask);

        var otherTask = new TaskItem("Other Project Task", otherProject, userId);
        // not completed on purpose

        await taskRepository.AddAsync(validTask);
        await taskRepository.AddAsync(otherTask);

        var result = await taskRepository
            .AreAllTasksCompletedForProjectIdAsync(targetProject);

        result.Should().BeTrue();
    }

 

    private void CompleteTaskTest(TaskItem task)
    {
        var userToAssignId = Guid.NewGuid();
        task.AssignTo(userToAssignId);
        task.StartWork();
        task.CompleteTask();
    }
}


