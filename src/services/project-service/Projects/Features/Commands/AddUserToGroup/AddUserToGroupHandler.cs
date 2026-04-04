namespace project_service.Projects.Features.Commands.AddUserToGroup;


public record AddUserToGroupCommand(Guid TargetUserId, Guid ProjectId, Guid OwnerId): IRequest<Unit>;
public class AddUserToGroupCommandValidator: AbstractValidator<AddUserToGroupCommand>
{
    public AddUserToGroupCommandValidator()
    {
        RuleFor(c=>c.TargetUserId)
        .NotEmpty().WithMessage("UserId is required");

        RuleFor(c=>c.ProjectId)
        .NotEmpty().WithMessage("ProjectId is required");
    }
}
public class AddUserToGroupHandler(IProjectRepository projectRepository,
    ILogger<AddUserToGroupHandler> logger) : IRequestHandler<AddUserToGroupCommand,Unit>
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<AddUserToGroupHandler> _logger = logger;
    public async Task<Unit> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling AddUserToProjectGroupCommand for ProjectId: {ProjectId}, UserId: {UserId}",
            request.ProjectId,
            request.TargetUserId);

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project is null)
        {
            _logger.LogWarning(
                "Project not found. ProjectId: {ProjectId}",
                request.ProjectId);

            throw new NotFoundException(nameof(Project),request.ProjectId.ToString());
        }
        if(project.OwnerId != request.OwnerId)
        {
            _logger.LogWarning(
                "User {UserId} is not the owner of the project {ProjectId}",
                request.OwnerId, request.ProjectId);

            throw new ForbiddenException(request.OwnerId.ToString(), "ADD_USER_TO_GROUP");
        }
        var wasAlreadyInGroup = project.IsInGroup(request.TargetUserId);

        project.AddUserToGroup(request.TargetUserId);

        if (wasAlreadyInGroup)
        {
            _logger.LogInformation(
                "User already in group. No changes applied. ProjectId: {ProjectId}, UserId: {UserId}",
                request.ProjectId,
                request.TargetUserId);
        }
        else
        {
            _logger.LogInformation(
                "User added to group successfully. ProjectId: {ProjectId}, UserId: {UserId}",
                request.ProjectId,
                request.TargetUserId);
        }

        await _projectRepository.EditAsync(project);

        _logger.LogInformation(
            "PROJECT_UPDATED successfully. ProjectId: {ProjectId}",
            request.ProjectId);

        return Unit.Value;
    }
}