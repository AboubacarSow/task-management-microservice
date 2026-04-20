namespace task_service.Tasks.Features.Commands.CompleteTask;

public record CompleteTaskCommand(Guid TaskId,Guid CurrentUserId,string? Notes=null):IRequest<Unit>;

public class CompleteTaskrequestValidator : AbstractValidator<CompleteTaskCommand>
{
    public CompleteTaskrequestValidator()
    {
        RuleFor(c=>c.TaskId).NotEmpty().WithMessage("Task Id is required");
        RuleFor(c=>c.CurrentUserId).NotEmpty().WithMessage("*user is empty*");
    }
}
public class CompleteTaskHandler(ITaskRepository taskRepository,ILogger<CompleteTaskHandler> logger)
: IRequestHandler<CompleteTaskCommand, Unit>
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ILogger<CompleteTaskHandler> _logger = logger;

    public async Task<Unit> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId)
        ?? throw new NotFoundException(nameof(TaskItem),request.TaskId.ToString());
        //Check if current user is assigned user
        if (task.AssignedToUser != request.CurrentUserId)
        {
            _logger.LogWarning(
                "User {CurrentUserId} tried to COMPLETE task not belong to him",
                request.CurrentUserId);
            throw new ForbiddenException(request.CurrentUserId.ToString(), "COMPLETE_TASK");
        }
        task.CompleteTask(request.Notes);

        await _taskRepository.EditAsync(task);

        _logger.LogInformation(
            "Task {TaskId} completed by {UserId}",
            task.Id, request.CurrentUserId);
        return Unit.Value;
    }
}