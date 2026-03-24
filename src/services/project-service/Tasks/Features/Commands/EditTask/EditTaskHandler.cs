

using project_service.Tasks.Dtos;

namespace project_service.Tasks.Features.Commands.EditTask;


public record EditTaskCommand(Guid CurrentUserId,
        Guid TaskId,
        string Title,
        DateTime DueAt,
        string? Description=null): IRequest<TaskItemDto>;
public class EditTaskHandler(ITaskRepository taskRepository,IProjectRepository projectRepository, 
ILogger<EditTaskHandler> logger)
: IRequestHandler<EditTaskCommand, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<EditTaskHandler> _logger = logger;

    public async Task<TaskItemDto> Handle(EditTaskCommand command, CancellationToken none)
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
                command.CurrentUserId, "EDIT_TASK");

            throw new ForbiddenException(command.CurrentUserId.ToString(), "EDIT_TASK");
        }

        var owner = task.CreatedByUser;

        task.SetName(command.Title);
        if(!string.IsNullOrWhiteSpace(command.Description)) 
            task.SetDescription(command.Description);
        task.SetDueAt(command.DueAt);

        await _taskRepository.EditAsync(task);

        if (owner != command.CurrentUserId)
            _logger.LogInformation("User in Group updated Task :{TaskId} successfully",
                command.TaskId);
        return task.Adapt<TaskItemDto>();

    }
}