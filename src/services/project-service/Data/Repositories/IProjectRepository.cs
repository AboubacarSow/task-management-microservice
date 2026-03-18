using project_service.Projects.Models;

namespace project_service.Data.Repositories;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task EditAsync(Project prt);
    Task<List<Project>> GetAllByUserId(Guid userId);
    Task<Project?> GetByIdAsync(Guid id);
}
