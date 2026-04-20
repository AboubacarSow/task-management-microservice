using task_service.Tasks.Grpc.Client;

namespace task_service.Tasks.Features.Commands.CreateTaskItem;


public record CreateTaskItemCommand(Guid CurrentUserId,Guid ProjectId, string Title): IRequest<Guid>;


public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand> 
{

    public CreateTaskItemCommandValidator() 
    {
        RuleFor(c => c.CurrentUserId).NotEmpty()
            .WithMessage("CurrentUserId is required");

        RuleFor(c => c.ProjectId).NotEmpty().WithMessage("ProjectId is required");

        RuleFor(c => c.Title).NotEmpty()
            .MaximumLength(200).WithMessage("Title to long or empty");
    }
}

public class CreateTaskItemHandler(
    ITaskRepository taskRepo,
    IProjectClient projectClient,
    ILogger<CreateTaskItemHandler> logger) : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskRepository _taskRepo = taskRepo;
    private readonly IProjectClient _projectClient = projectClient;
    private readonly ILogger<CreateTaskItemHandler> _logger = logger;

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {

        _logger.LogInformation(
            "User {UserId} creating task in Project {ProjectId}",
            request.CurrentUserId, request.ProjectId);

        var projectModel = await _projectClient
            .GetProjectAsync(request.ProjectId.ToString());

        if (projectModel is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            throw new NotFoundException("Project", request.ProjectId.ToString());
        }
         var isMember = projectModel.Group
            .Any(g => Guid.Parse(g) == request.CurrentUserId);
        if (!isMember && Guid.Parse(projectModel.OwnerId) != request.CurrentUserId)
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


        return task.Id;
    }
}
