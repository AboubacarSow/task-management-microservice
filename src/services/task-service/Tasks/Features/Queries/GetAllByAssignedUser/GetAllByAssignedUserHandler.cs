namespace task_service.Tasks.Features.Queries.GetAllByAssignedUser;

public record GetAllByAssignedUserQuery(Guid CurrentUserId):IRequest<List<TaskItemDto>>;

public class GetAllByAssignedUserHandler(ITaskRepository repository,
 ILogger<GetAllByAssignedUserHandler> logger) : IRequestHandler<GetAllByAssignedUserQuery, List<TaskItemDto>>
{
    private readonly ITaskRepository _repository = repository;
    private readonly ILogger<GetAllByAssignedUserHandler> _logger = logger;

    public async Task<List<TaskItemDto>> Handle(GetAllByAssignedUserQuery query, CancellationToken none)
    {
        var tasks = await _repository.GetAllByAssignedUserIdAsync(query.CurrentUserId);

        if (!tasks.Any())
        {
            _logger.LogWarning("No tasks for owner {OwnerId}", query.CurrentUserId);
            return [];
        }

        _logger.LogInformation("{Count} tasks for assigned_user {CurrentUserId}", tasks.Count, query.CurrentUserId);

        return tasks.Adapt<List<TaskItemDto>>(options =>
        {
            options.ForType<TaskItem, TaskItemDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        });
    }
}
