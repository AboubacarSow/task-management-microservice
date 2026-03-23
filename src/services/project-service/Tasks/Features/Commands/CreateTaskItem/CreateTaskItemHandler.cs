

namespace project_service.Tasks.Features.Commands.CreateTaskItem;


public record CreateTaskItemCommand(Guid CurrentUserId,Guid ProjectId, string Title): IRequest<Guid>;

public class CreateTaskItemHandler(
    ITaskRepository taskRepo,
    IProjectRepository projectRepo,
    ILogger<CreateTaskItemHandler> logger) : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskRepository _taskRepo = taskRepo;
    private readonly IProjectRepository _projectRepo = projectRepo;
    private readonly ILogger<CreateTaskItemHandler> _logger = logger;

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {

        _logger.LogInformation(
            "User {UserId} creating task in Project {ProjectId}",
            request.CurrentUserId, request.ProjectId);

        var project = await _projectRepo.GetByIdAsync(request.ProjectId);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            throw new NotFoundException(nameof(Project), request.ProjectId.ToString());
        }

        if (!project.IsInGroup(request.CurrentUserId))
        {
            _logger.LogWarning(
                "User {UserId} is not allowed to create task in Project {ProjectId}",
                request.CurrentUserId, request.ProjectId);

            throw new ForbiddenException(request.CurrentUserId.ToString(), "CREATE_TASK");
        }

        var task = new TaskItem(request.Title, request.ProjectId, request.CurrentUserId);

        await _taskRepo.AddAsync(task);

        _logger.LogInformation(
            "Task {TaskId} created successfully in Project {ProjectId}",
            task.Id, task.ProjectId);

        // ToDo
        //Raise TaskCreatedEvent()

        return task.Id;
    }
}