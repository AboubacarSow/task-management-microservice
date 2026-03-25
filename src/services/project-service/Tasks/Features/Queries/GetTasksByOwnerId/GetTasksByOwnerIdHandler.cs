
using project_service.Tasks.Dtos;

namespace project_service.Tasks.Features.Queries.GetTasksByOwnerId;

public record GetTasksByOwnerIdQuery(Guid CurrentUserId);

public class GetTasksByOwnerIdHandler(ITaskRepository taskRepository,
ILogger<GetTasksByOwnerIdHandler> logger)
{
    private readonly ITaskRepository _taskRepository = taskRepository ;
    private readonly ILogger<GetTasksByOwnerIdHandler> _logger = logger;

    public async Task<List<TaskItemDto>> Handle(GetTasksByOwnerIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}