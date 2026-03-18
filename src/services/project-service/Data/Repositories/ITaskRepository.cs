using Task = project_service.Tasks.Models.Task;

namespace project_service.Data.Repositories;

public interface ITaskRepository
{
    System.Threading.Tasks.Task AddAsync(Task task);
    Task<List<Task>> GetAllByProjectId(Guid projectId);
    Task<List<Task>> GetAllByUserIdAsync(Guid userId);
    System.Threading.Tasks.Task<Task?> GetByIdAsync(Guid id);
}
