<<<<<<< HEAD


namespace task_service.Data.Repositories;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task<bool> AreAllTasksCompletedForProjectIdAsync(Guid projectId);
    Task EditAsync(TaskItem oldTask);
    Task<List<TaskItem>> GetAllByProjectIdAsync(Guid projectId);
    Task<List<TaskItem>> GetAllByOwnerIdAsync(Guid userId);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetAllByAssignedUserIdAsync(Guid user2);
}
public class TaskRepository(IMongoCollection<TaskItem> collection) : ITaskRepository
{
    private readonly IMongoCollection<TaskItem> _collection = collection;

    public Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return _collection.Find(t => t.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public async Task AddAsync(TaskItem task)
    {
        await _collection.InsertOneAsync(task);
    }

    public Task<List<TaskItem>> GetAllByProjectIdAsync(Guid projectId)
    {
        return _collection.Find(t => t.ProjectId == projectId)
                          .ToListAsync();
    }

    public Task<List<TaskItem>> GetAllByOwnerIdAsync(Guid userId)
    => _collection.Find(t => t.CreatedByUser == userId)
                          .ToListAsync();

    public async Task EditAsync(TaskItem task)
    {
        await _collection.ReplaceOneAsync(t=>t.Id==task.Id,
        task);
    }

    public async Task<bool> AreAllTasksCompletedForProjectIdAsync(Guid projectId)
    {
        var hasIncomplete = await _collection
        .Find(t => t.ProjectId == projectId && !(t.Status==Tasks.Models.TaskStatus.Completed))
        .AnyAsync();

        return !hasIncomplete;
    }

    public Task<List<TaskItem>> GetAllByAssignedUserIdAsync(Guid userId)
      => _collection.Find(t => t.AssignedToUser == userId)
                    .ToListAsync();
}
=======


namespace task_service.Data.Repositories;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task<bool> AreAllTasksCompletedForProjectIdAsync(Guid projectId);
    Task EditAsync(TaskItem oldTask);
    Task<List<TaskItem>> GetAllByProjectIdAsync(Guid projectId);
    Task<List<TaskItem>> GetAllByOwnerIdAsync(Guid userId);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetAllByAssignedUserIdAsync(Guid user2);
}
public class TaskRepository(IMongoCollection<TaskItem> collection) : ITaskRepository
{
    private readonly IMongoCollection<TaskItem> _collection = collection;

    public Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return _collection.Find(t => t.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public async Task AddAsync(TaskItem task)
    {
        await _collection.InsertOneAsync(task);
    }

    public Task<List<TaskItem>> GetAllByProjectIdAsync(Guid projectId)
    {
        return _collection.Find(t => t.ProjectId == projectId)
                          .ToListAsync();
    }

    public Task<List<TaskItem>> GetAllByOwnerIdAsync(Guid userId)
    => _collection.Find(t => t.CreatedByUser == userId)
                          .ToListAsync();

    public async Task EditAsync(TaskItem task)
    {
        await _collection.ReplaceOneAsync(t=>t.Id==task.Id,
        task);
    }

    public async Task<bool> AreAllTasksCompletedForProjectIdAsync(Guid projectId)
    {
        var hasIncomplete = await _collection
        .Find(t => t.ProjectId == projectId && !(t.Status==Tasks.Models.TaskStatus.Completed))
        .AnyAsync();

        return !hasIncomplete;
    }

    public Task<List<TaskItem>> GetAllByAssignedUserIdAsync(Guid userId)
      => _collection.Find(t => t.AssignedToUser == userId)
                    .ToListAsync();
}
>>>>>>> 05b451b (new_update)
