using MongoDB.Driver;
using project_service.Projects.Models;

namespace project_service.Data.Repositories;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<Project> GetByIdAsync(Guid id);
}

public class ProjectRepository(IMongoCollection<Project> collection) : IProjectRepository
{
    private readonly IMongoCollection<Project> _collection = collection;

    public async Task AddAsync(Project project)
    {
        await _collection.InsertOneAsync(project);
    }

    public async Task<Project> GetByIdAsync(Guid id)
    {
        return await _collection.Find(p=>p.Id==id).FirstOrDefaultAsync();
    }
}