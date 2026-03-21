using MongoDB.Driver;
using project_service.Data.Repositories;
using project_service.Projects.Models;
using TaskItem = project_service.Tasks.Models.TaskItem;

namespace project_service.Tests.Helpers;

public static class FakeRepositories
{
    public static IProjectRepository GetProjectRepository(IMongoCollection<Project> collection)
    {
        return new ProjectRepository(collection);
    }

    public static ITaskRepository GetTaskRepository(IMongoCollection<TaskItem> collection)
        => new TaskRepository(collection);

}