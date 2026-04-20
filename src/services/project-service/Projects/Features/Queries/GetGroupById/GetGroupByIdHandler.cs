
namespace project_service.Projects.Features.Queries.GetGroupById;


public record GetGroupByIdQuery(Guid UserId,Guid ProjectId):IRequest<List<Guid>>;

public class GetGroupByIdQueryValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdQueryValidator()
    {
        RuleFor(q => q.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(q => q.ProjectId).NotEmpty().WithMessage("ProjectId is required");
    }
}
public class GetGroupByIdHandler(IProjectRepository repository, ILogger<GetGroupByIdHandler> logger)
        : IRequestHandler<GetGroupByIdQuery, List<Guid>>
{
    private readonly IProjectRepository _repository = repository;
    private readonly ILogger<GetGroupByIdHandler> _logger = logger;

    public async Task<List<Guid>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} is requesting Group for Project {ProjectId}",
            request.UserId,
            request.ProjectId);

        var project = await _repository.GetByIdAsync(request.ProjectId);

        if (project is null)
        {
            _logger.LogWarning(
                "Project not found. ProjectId: {ProjectId}",
                request.ProjectId);

            throw new NotFoundException(nameof(Project), request.ProjectId.ToString());
        }

        if (request.UserId != project.OwnerId)
        {
            _logger.LogWarning(
                "Unauthorized access to Group. User {UserId} is not owner of Project {ProjectId}",
                request.UserId,
                request.ProjectId);

            throw new UnauthorizedAccessException();
        }

        _logger.LogInformation(
            "Group retrieved successfully for Project {ProjectId} by Owner {UserId}",
            request.ProjectId,
            request.UserId);


        return [.. project.Group];
    }
}
