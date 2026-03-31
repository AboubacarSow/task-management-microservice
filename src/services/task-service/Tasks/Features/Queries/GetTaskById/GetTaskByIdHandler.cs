using task_service.Commons.Exceptions;

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
public class GetTaskByIdHandler(ITaskRepository taskRepository, //IProjectRepository projectRepository,
 ILogger<GetTaskByIdHandler> logger): IRequestHandler<GetTaskByIdQuery, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    //private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<GetTaskByIdHandler> _logger = logger;

    

    public async Task<TaskItemDto> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(query.TaskId);

        if (task is null)
        {
            _logger.LogWarning("Task with Id:{TaskId} not found", query.TaskId);
            throw new NotFoundException(nameof(Task), query.TaskId.ToString());
        }

        //var project = await _projectRepository.GetByIdAsync(task.ProjectId);

        /* if (project == null)
        {
            _logger.LogWarning("No project found for Task with Id:{TaskId}", query.TaskId);
            throw new NotFoundException(nameof(Project), task.ProjectId.ToString());
        }
        */
        //var isInPeople = project.IsInPeopleWorking(query.CurrentUserId);
        //if (!isInPeople)
        //{
        //    _logger.LogWarning(
        //        "User {UserId} not allowed to access to this resource",
        //        query.CurrentUserId);
        //    throw new ForbiddenException(query.CurrentUserId.ToString(), "READ_TASK");
        //}

        return task.Adapt<TaskItemDto>();
    }
}