using MongoDB.Driver;
using Task = project_service.Tasks.Models.Task;

namespace project_service.Data.Repositories;

public class TaskRepository(IMongoCollection<Task> collection) : ITaskRepository
{
    private readonly IMongoCollection<Task> _collection = collection;

    public Task<Task?> GetByIdAsync(Guid id)
    {
        return _collection.Find(t => t.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public async System.Threading.Tasks.Task AddAsync(Task task)
    {
        await _collection.InsertOneAsync(task);
    }

    public Task<List<Task>> GetAllByProjectId(Guid projectId)
    {
        return _collection.Find(t => t.ProjectId == projectId)
                          .ToListAsync();
    }

    public Task<List<Task>> GetAllByUserIdAsync(Guid userId)
    => _collection.Find(t => t.CreatedByUser == userId)
                          .ToListAsync();
}
