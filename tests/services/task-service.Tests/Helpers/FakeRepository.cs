<<<<<<< HEAD
using MongoDB.Driver;
using task_service.Data.Repositories;
using task_service.Tasks.Models;

namespace task_service.Tests.Helpers;

public static class FakeRepository
{
    public static ITaskRepository GetTaskRepository(IMongoCollection<TaskItem> collection)
        => new TaskRepository(collection);
=======
using MongoDB.Driver;
using task_service.Data.Repositories;
using task_service.Tasks.Models;

namespace task_service.Tests.Helpers;

public static class FakeRepository
{
    public static ITaskRepository GetTaskRepository(IMongoCollection<TaskItem> collection)
        => new TaskRepository(collection);
>>>>>>> 05b451b (new_update)
}