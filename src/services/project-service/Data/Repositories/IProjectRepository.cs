using MongoDB.Driver;
using project_service.Projects.Models;

namespace project_service.Data.Repositories;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<Project> GetByIdAsync(Guid id);
}

public class ProjectRepository : IProjectRepository
{
    public ProjectRepository(IMongoCollection<Project> mongoCollection)
    {
    }

    public Task AddAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task<Project> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}