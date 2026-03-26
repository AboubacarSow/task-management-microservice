
using project_service.Tasks.Dtos;

namespace project_service.Tasks.Features.Queries.GetTasksByOwnerId;

public record GetTasksByOwnerIdQuery(Guid CurrentUserId):IRequest<List<TaskItemDto>>;

public class GetTasksByOwnerIdHandler(ITaskRepository taskRepository,
    ILogger<GetTasksByOwnerIdHandler> logger) : IRequestHandler<GetTasksByOwnerIdQuery, List<TaskItemDto>>
{
    private readonly ITaskRepository _taskRepository = taskRepository ;
    private readonly ILogger<GetTasksByOwnerIdHandler> _logger = logger;

    public async Task<List<TaskItemDto>> Handle(GetTasksByOwnerIdQuery query, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllByOwnerIdAsync(query.CurrentUserId);

        if (!tasks.Any())
        {
            _logger.LogWarning("No tasks for owner {OwnerId}", query.CurrentUserId);
            return [];
        }

        _logger.LogInformation("{Count} tasks for owner {OwnerId}", tasks.Count, query.CurrentUserId);

        return tasks.Adapt<List<TaskItemDto>>();
    }

 
}