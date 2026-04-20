<<<<<<< HEAD
using task_service.Commons.Exceptions;
using task_service.Tasks.Grpc.Client;

namespace task_service.Tasks.Features.Queries.GetTaskById;


public record GetTaskByIdQuery(Guid CurrentUserId,Guid TaskId):IRequest<TaskItemDto>;


public class GetTaskByIdQueryValidator: AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty()
            .WithMessage("Task Id is required");
        RuleFor(x => x.CurrentUserId).NotEmpty()
            .WithMessage("User is required");
    }
}
public class GetTaskByIdHandler(ITaskRepository taskRepository, ProjectClient projectClient,
 ILogger<GetTaskByIdHandler> logger): IRequestHandler<GetTaskByIdQuery, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ProjectClient _projectClient = projectClient;
    private readonly ILogger<GetTaskByIdHandler> _logger = logger;

    

    public async Task<TaskItemDto> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(query.TaskId);

        if (task is null)
        {
            _logger.LogWarning("Task with Id:{TaskId} not found", query.TaskId);
            throw new NotFoundException(nameof(Task), query.TaskId.ToString());
        }

        var projectModel = await _projectClient
                            .GetProjectAsync(task.ProjectId.ToString());

        if (projectModel == null)
        {
            _logger.LogWarning("No project found for Task with Id:{TaskId}", query.TaskId);
            throw new NotFoundException("Project", task.ProjectId.ToString());
        }
        
        var isInPeople = projectModel.PeopleWorking
            .Any(g => Guid.Parse(g) == query.CurrentUserId);
        if (!isInPeople)
        {
            _logger.LogWarning(
                "User {UserId} not allowed to access to this resource",
                query.CurrentUserId);
            throw new ForbiddenException(query.CurrentUserId.ToString(), "READ_TASK");
        }

        return task.Adapt<TaskItemDto>(options =>
        {
            options.ForType<TaskItem, TaskItemDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        });
    }
=======
using task_service.Commons.Exceptions;
using task_service.Tasks.Grpc.Client;

namespace task_service.Tasks.Features.Queries.GetTaskById;


public record GetTaskByIdQuery(Guid CurrentUserId,Guid TaskId):IRequest<TaskItemDto>;


public class GetTaskByIdQueryValidator: AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty()
            .WithMessage("Task Id is required");
        RuleFor(x => x.CurrentUserId).NotEmpty()
            .WithMessage("User is required");
    }
}
public class GetTaskByIdHandler(ITaskRepository taskRepository, IProjectClient projectClient,
 ILogger<GetTaskByIdHandler> logger): IRequestHandler<GetTaskByIdQuery, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectClient _projectClient = projectClient;
    private readonly ILogger<GetTaskByIdHandler> _logger = logger;

    

    public async Task<TaskItemDto> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(query.TaskId);

        if (task is null)
        {
            _logger.LogWarning("Task with Id:{TaskId} not found", query.TaskId);
            throw new NotFoundException(nameof(Task), query.TaskId.ToString());
        }

        var projectModel = await _projectClient
                            .GetProjectAsync(task.ProjectId.ToString());

        if (projectModel == null)
        {
            _logger.LogWarning("No project found for Task with Id:{TaskId}", query.TaskId);
            throw new NotFoundException("Project", task.ProjectId.ToString());
        }
        
        var isInPeople = projectModel.PeopleWorking
            .Any(g => Guid.Parse(g) == query.CurrentUserId);
        if (!isInPeople)
        {
            _logger.LogWarning(
                "User {UserId} not allowed to access to this resource",
                query.CurrentUserId);
            throw new ForbiddenException(query.CurrentUserId.ToString(), "READ_TASK");
        }

        return task.Adapt<TaskItemDto>(options =>
        {
            options.ForType<TaskItem, TaskItemDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        });
    }
>>>>>>> 05b451b (new_update)
}