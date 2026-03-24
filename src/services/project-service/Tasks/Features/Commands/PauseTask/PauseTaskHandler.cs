

namespace project_service.Tasks.Features.Commands.PauseTask;


public record PauseTaskCommand(Guid TaskId, Guid UserId, string Notes): IRequest<Unit>;
public class PauseTaskHandler(ITaskRepository taskRepository, IProjectRepository projectRepository,
 ILogger<PauseTaskHandler> logger) : IRequestHandler<PauseTaskCommand, Unit>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<PauseTaskHandler> _logger = logger;
    public async Task<Unit> Handle(PauseTaskCommand command, CancellationToken none)
    {
        var task = await _taskRepository.GetByIdAsync(command.TaskId);

        if(task == null)
        {
            _logger.LogWarning(
                "Task with Id:{TaskId} NOT_FOUND",
                command.TaskId);
            throw new NotFoundException(nameof(TaskItem),
                command.TaskId.ToString());
        }

        if (task.AssignedToUser != command.UserId)
        {
            _logger.LogWarning(
                "*only assigned user is allowed perform this operation:{operation}",
                "PAUSE_TASK");
            throw new ForbiddenException(command.UserId.ToString(),"PAUSE_TASK");
        }

        task.Pause(command.Notes);

        await _taskRepository.EditAsync(task);
        _logger.LogInformation("Task with Id:{TaskId} is paused at {date}",
            command.TaskId, DateTime.UtcNow.ToString());
        return Unit.Value;
    }
}
