namespace project_service.Projects.Features.Commands.AddUserToPeople;


public record AddUserToPeopleWorkingCommand(Guid ProjectId, Guid TargetUserId): IRequest<Unit>;

public class AddUserToPeopleWorkingCommandValidator : AbstractValidator<AddUserToPeopleWorkingCommand>
{
    public AddUserToPeopleWorkingCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.TargetUserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}

public class AddUserToPeopleWorkingHandler(
    IProjectRepository projectRepository,
    ILogger<AddUserToPeopleWorkingHandler> logger)
        : IRequestHandler<AddUserToPeopleWorkingCommand,Unit>
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<AddUserToPeopleWorkingHandler> _logger = logger;

    public async Task<Unit> Handle(
        AddUserToPeopleWorkingCommand request, 
        CancellationToken cancellationToken)
    {

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);

        if (project is null)
        {
            _logger.LogWarning(
                "Project not found. ProjectId: {ProjectId}",
                request.ProjectId);

            throw new NotFoundException(nameof(project),request.ProjectId.ToString());
        }

        var alreadyWorking = project.PeopleWorking.Contains(request.TargetUserId);

        project.AddToPeopleWorking(request.TargetUserId);

        if (alreadyWorking)
        {
            _logger.LogInformation(
                "User already in PeopleWorking. No changes applied. ProjectId: {ProjectId}, UserId: {UserId}",
                request.ProjectId,
                request.TargetUserId);
        }
        else
        {
            _logger.LogInformation(
                "User added to PeopleWorking. ProjectId: {ProjectId}, UserId: {UserId}",
                request.ProjectId,
                request.TargetUserId);
        }

        await _projectRepository.EditAsync(project);
        _logger.LogInformation(
            "Project updated successfully after PeopleWorking change. ProjectId: {ProjectId}",
            request.ProjectId);

        return Unit.Value;
    }
}