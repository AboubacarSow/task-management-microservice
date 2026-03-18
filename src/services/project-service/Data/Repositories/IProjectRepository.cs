using MongoDB.Driver;
using MongoDB.Driver.Linq;
using project_service.Projects.Models;

namespace project_service.Data.Repositories;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<List<Project>> GetAllByUserId(Guid userId);
    Task<Project> GetByIdAsync(Guid id);
}

public class ProjectRepository(IMongoCollection<Project> collection) : IProjectRepository
{
    private readonly IMongoCollection<Project> _collection = collection;

    public async Task AddAsync(Project project)
    {
        await _collection.InsertOneAsync(project);
    }

    public async Task<List<Project>> GetAllByUserId(Guid userId)
    {
        return await _collection.Find(p=>p.CreatedByUser==userId)
                            .ToListAsync();
    }

    public async Task<Project> GetByIdAsync(Guid id)
    {
        return await _collection.Find(p=>p.Id==id).FirstOrDefaultAsync();
    }
}