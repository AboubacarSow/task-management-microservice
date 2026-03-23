namespace project_service.Projects.Features.Commands.EditProjectState;


public record EditProjectStateCommand(Guid ProjectId,
    Guid UserId,
    ProjectStatus Status): IRequest;

public class EditProjectStateCommandValidator : AbstractValidator<EditProjectStateCommand> {

    public EditProjectStateCommandValidator()
    {

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");


        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid project state.");

      
    }
 }

public sealed class EditProjectStateHandler(
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    ILogger<EditProjectStateHandler> logger)
        : IRequestHandler<EditProjectStateCommand>
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ILogger<EditProjectStateHandler> _logger = logger;

    public async Task Handle(
        EditProjectStateCommand request,
        CancellationToken cancellationToken)
    {

        var project = await _projectRepository
            .GetByIdAsync(request.ProjectId);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} NOT_FOUND", request.ProjectId);
            throw new NotFoundException(nameof(Project),request.ProjectId.ToString());
        }

        if (project.OwnerId != request.UserId)
        {
            _logger.LogWarning(
                "User {UserId}  IS_NOT_OWNER of project {ProjectId}",
                request.UserId,
                request.ProjectId);

            throw new ForbiddenException(
                request.UserId.ToString(),"MODIFY_PROJECT");
        }

        if (request.Status == ProjectStatus.Completed)
        {
            var allTasksCompleted = await _taskRepository
                .AreAllTasksCompletedForProjectIdAsync(project.Id);

            if (!allTasksCompleted)
            {
                _logger.LogWarning(
                    "Project {ProjectId} CAN_NOT_BE_MARKED_AS_COMPLETED because tasks are incomplete",
                    project.Id);

                throw new DomainException("All tasks must be completed before completing the project");
            }
        }

        switch (request.Status)
        {
            case ProjectStatus.Completed:
                project.Complete();
                break;

            case ProjectStatus.OnHold:
                project.PutOnHold();
                break;

            case ProjectStatus.Active:
                project.Reactivate();
                break;

            case ProjectStatus.Archived:
                project.Archive();
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(request.Status),
                    request.Status,
                    "Invalid project status");
        }


        await _projectRepository.EditAsync(project);

        _logger.LogInformation(
            "Project {ProjectId} state changed to {Status} by user {UserId}",
            project.Id,
            project.Status,
            request.UserId);
    }
}


