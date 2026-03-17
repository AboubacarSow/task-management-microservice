using MongoDB.Driver;
using project_service.Data.Repositories;
using project_service.Projects.Models;

namespace project_service.Tests.Helpers;

public static class FakeRepositories
{
    public static IProjectRepository GetProjectRepository(IMongoCollection<Project> collection)
    {
        return new ProjectRepository(collection);
    }

}