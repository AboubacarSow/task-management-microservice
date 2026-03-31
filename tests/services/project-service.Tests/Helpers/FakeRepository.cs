using MongoDB.Driver;

namespace project_service.Tests.Helpers;

public static class FakeRepository
{
    public static IProjectRepository GetProjectRepository(IMongoCollection<Project> collection)
    {
        return new ProjectRepository(collection);
    }

  

}