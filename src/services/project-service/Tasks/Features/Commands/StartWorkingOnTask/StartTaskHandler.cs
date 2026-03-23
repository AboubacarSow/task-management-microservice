

namespace project_service.Tasks.Features.Commands.StartWorkingOnTask;


public record StartTaskCommand(Guid TaskId,Guid CurrentUserId):IRequest<Unit>;
public class StartTaskHandler : IRequestHandler<StartTaskCommand,Unit>
{
    private readonly ITaskRepository _taskRepo;
    private readonly ILogger<StartTaskHandler> _logger;

    public StartTaskHandler(
        ITaskRepository taskRepo,
        ILogger<StartTaskHandler> logger)
    {
        _taskRepo = taskRepo;
        _logger = logger;
    }

    public async Task<Unit> Handle(StartTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepo.GetByIdAsync(request.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId.ToString());

        if (task.AssignedToUser != request.CurrentUserId)
        {
            _logger.LogWarning(
                "User {UserId} cannot start task {TaskId}",
                request.CurrentUserId, task.Id);

            throw new ForbiddenException(request.CurrentUserId.ToString(), "START_TASK");
        }

        task.StartWork();

        await _taskRepo.EditAsync(task);

        _logger.LogInformation(
            "Task {TaskId} started by {UserId}",
            task.Id, request.CurrentUserId);

        return Unit.Value;
    }
}