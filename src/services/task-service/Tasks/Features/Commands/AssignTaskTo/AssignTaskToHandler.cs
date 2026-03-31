
namespace task_service.Tasks.Features.Commands.AssignTaskTo;


public record AssignTaskToCommand(Guid TaskId,Guid CurrentUserId,Guid UserId):IRequest<Unit>;

public class AssignTaskToCommandValidator : AbstractValidator<AssignTaskToCommand>
{
    public AssignTaskToCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("TaskId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserIs is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty();
    }
}

public class AssignTaskToHandler(ITaskRepository taskRepository, //IProjectRepository projectRepository, 
ILogger<AssignTaskToHandler> logger):IRequestHandler<AssignTaskToCommand,Unit>
{
    public readonly ITaskRepository _taskRepository = taskRepository;
    //public readonly IProjectRepository _projectRepository = projectRepository;
    public readonly ILogger<AssignTaskToHandler> _logger  = logger;

    public async Task<Unit> Handle(AssignTaskToCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId.ToString());

        //var project = await _projectRepository.GetByIdAsync(task.ProjectId)
           // ?? throw new NotFoundException(nameof(Project), task.ProjectId.ToString());

        //if (!project.IsInGroup(request.CurrentUserId))
        //{
         //   _logger.LogWarning(
        //        "User {UserId} cannot assign task {TaskId}",
        //        request.CurrentUserId, task.Id);
        //    throw new ForbiddenException(request.CurrentUserId.ToString(), "ASSIGN_TASK");
        //}

        task.AssignTo(request.UserId);

        //project.AddToPeopleWorking(request.UserId);

        await _taskRepository.EditAsync(task);
        //await _projectRepository.EditAsync(project);

        _logger.LogInformation(
            "Task {TaskId} assigned to {UserId} and added to PeopleWorking",
            task.Id, request.UserId);

        // (event later)
        // raise taskAssignedUser;

        return Unit.Value;
    }
}
