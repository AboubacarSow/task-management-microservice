

namespace project_service.Tasks.Features.Commands.CancelTask;

public record CancelTaskCommand(Guid CurrentUserId, Guid TaskId):IRequest<Unit>;
public class CancelTaskCommandValidator: AbstractValidator<CancelTaskCommand>
{
    public CancelTaskCommandValidator()
    {
        RuleFor(c=>c.CurrentUserId).NotEmpty();
        RuleFor(c=>c.TaskId).NotEmpty().WithMessage("*TaskId is required*");
    }
}

public class CancelTaskHandler(ITaskRepository taskRepo,IProjectRepository projectRepo,
ILogger<CancelTaskHandler> logger) : IRequestHandler<CancelTaskCommand,Unit>
{
   private readonly ITaskRepository _taskRepository = taskRepo;
   private readonly IProjectRepository _projectRepository = projectRepo;
   private readonly ILogger<CancelTaskHandler> _logger = logger;

    public async Task<Unit> Handle(CancelTaskCommand command, CancellationToken none)
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

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
        if(project == null)
        {
            _logger.LogWarning(
                "Project with Id:{ProjectId} NOT_FOUND for Task with Id : {TaskId}",
                task.ProjectId,task.Id);

            throw new NotFoundException(nameof(Project),task.ProjectId.ToString());
        }

        if (task.CreatedByUser !=  command.CurrentUserId &&
            !project.IsInGroup(command.CurrentUserId))
        {
            _logger.LogWarning(
                "User {UserId} not authorized to perform this operation :{Operation}",
                command.CurrentUserId, "CANCEL_TASK");

            throw new ForbiddenException(command.CurrentUserId.ToString(), "CANCEL_TASK");
        }

        var owner = task.CreatedByUser;

        task.Cancel();

        await _taskRepository.EditAsync(task);

        if (owner != command.CurrentUserId)
            _logger.LogInformation("User in Group cancelled Task :{TaskId} successfully",
                command.TaskId);
                
        return Unit.Value;
    }
}
