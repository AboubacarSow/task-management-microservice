namespace task_service.Tasks.Features.Commands.UnAssignTask;


public record UnAssignTaskCommand(Guid TaskId,Guid CurrentUserId):IRequest<Unit>;
public class UnAssignTaskHandler(ITaskRepository taskRepository,ILogger<UnAssignTaskHandler> logger)
    : IRequestHandler<UnAssignTaskCommand, Unit>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ILogger<UnAssignTaskHandler> _logger = logger;
    public async Task<Unit> Handle(UnAssignTaskCommand command, CancellationToken none)
    {
        var task = await _taskRepository.GetByIdAsync(command.TaskId);

        if(task == null)
        {
            _logger.LogWarning("Task with Id:{TaskId} not found",command.TaskId);
            throw new NotFoundException(nameof(command), command.TaskId.ToString());
        }
        if(task.CreatedByUser != command.CurrentUserId)
        {
            _logger.LogWarning("User {UserId} not allowed to perform UNASSIGN_TASK",command.CurrentUserId);
            throw new ForbiddenException(command.CurrentUserId.ToString(), "UNASSIGN_TASK");
        }
        var old_user = task.AssignedToUser;
        task.UnAssign();
        await _taskRepository.EditAsync(task);

        _logger.LogInformation("Task unassigned from User :{UserId} successfully", old_user);
        return Unit.Value;
    }
}