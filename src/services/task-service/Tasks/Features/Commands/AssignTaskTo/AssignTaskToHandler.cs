<<<<<<< HEAD

using MassTransit;
using shared.messaging.Events;
using System.Linq;
using task_service.Tasks.Grpc.Client;

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

public class AssignTaskToHandler(ITaskRepository taskRepository,
 ProjectClient projectClient, IPublishEndpoint publishEndpoint,
ILogger<AssignTaskToHandler> logger):IRequestHandler<AssignTaskToCommand,Unit>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ProjectClient _projectClient = projectClient;
    public readonly ILogger<AssignTaskToHandler> _logger  = logger;

    public async Task<Unit> Handle(AssignTaskToCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId.ToString());

        var projectModel = await _projectClient
            .GetProjectAsync(task.ProjectId.ToString())
            ?? throw new NotFoundException("Project", task.ProjectId.ToString());

        var isMember = projectModel.Group
            .Any(g => Guid.Parse(g) == request.CurrentUserId);
        if (!isMember && Guid.Parse(projectModel.OwnerId) != request.CurrentUserId)
        {
           _logger.LogWarning(
              "User {UserId} cannot assign task {TaskId}",
               request.CurrentUserId, task.Id);
            throw new ForbiddenException(request.CurrentUserId.ToString(), "ASSIGN_TASK");
        }

        task.AssignTo(request.UserId);


        await _taskRepository.EditAsync(task);

        await publishEndpoint.Publish(new TaskAssignedIntegrationEvent
        {
            TaskId = task.Id,
            ProjectId = task.ProjectId,
            AssignedUserId = request.UserId,
            AssignedAt = DateTime.UtcNow
        }, cancellationToken);

        _logger.LogInformation(
            "Task {TaskId} assigned to {UserId} and added to PeopleWorking",
            task.Id, request.UserId);

        

        return Unit.Value;
    }
}
=======

using MassTransit;
using shared.messaging.Events;
using System.Linq;
using task_service.Tasks.Grpc.Client;

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

public class AssignTaskToHandler(ITaskRepository taskRepository,
 IProjectClient projectClient, IPublishEndpoint publishEndpoint,
ILogger<AssignTaskToHandler> logger):IRequestHandler<AssignTaskToCommand,Unit>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectClient _projectClient = projectClient;
    public readonly ILogger<AssignTaskToHandler> _logger  = logger;

    public async Task<Unit> Handle(AssignTaskToCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId.ToString());

        var projectModel = await _projectClient
            .GetProjectAsync(task.ProjectId.ToString())
            ?? throw new NotFoundException("Project", task.ProjectId.ToString());

        var isMember = projectModel.Group
            .Any(g => Guid.Parse(g) == request.CurrentUserId);
        if (!isMember && Guid.Parse(projectModel.OwnerId) != request.CurrentUserId)
        {
           _logger.LogWarning(
              "User {UserId} cannot assign task {TaskId}",
               request.CurrentUserId, task.Id);
            throw new ForbiddenException(request.CurrentUserId.ToString(), "ASSIGN_TASK");
        }

        task.AssignTo(request.UserId);


        await _taskRepository.EditAsync(task);

        await publishEndpoint.Publish(new TaskAssignedIntegrationEvent
        {
            TaskId = task.Id,
            ProjectId = task.ProjectId,
            AssignedUserId = request.UserId,
            AssignedAt = DateTime.UtcNow
        }, cancellationToken);

        _logger.LogInformation(
            "Task {TaskId} assigned to {UserId} and added to PeopleWorking",
            task.Id, request.UserId);

        

        return Unit.Value;
    }
}
>>>>>>> 05b451b (new_update)
