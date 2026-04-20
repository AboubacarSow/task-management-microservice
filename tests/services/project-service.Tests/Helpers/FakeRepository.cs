<<<<<<< HEAD
using MongoDB.Driver;

namespace project_service.Tests.Helpers;

public static class FakeRepository
{
    public static IProjectRepository GetProjectRepository(IMongoCollection<Project> collection)
    {
        return new ProjectRepository(collection);
    }

  

=======
using MongoDB.Driver;

namespace project_service.Tests.Helpers;

public static class FakeRepository
{
    public static IProjectRepository GetProjectRepository(IMongoCollection<Project> collection)
    {
        return new ProjectRepository(collection);
    }

  

>>>>>>> 05b451b (new_update)
}