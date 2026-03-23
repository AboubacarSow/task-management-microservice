namespace project_service.Projects.Features.Queries.GetProjectsByOwnerId;


public record GetProjectsByOwnerIdQuery(Guid OwnerId):IRequest<List<ProjectDto>>;

public class GetProjectsByOwnerIdQueryValidator 
    : AbstractValidator<GetProjectsByOwnerIdQuery>
{
    public GetProjectsByOwnerIdQueryValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID is required.");
    }
}
public class GetProjectsByOwnerIdHandler(IProjectRepository repository, ILogger<GetProjectsByOwnerIdHandler> logger) : IRequestHandler<GetProjectsByOwnerIdQuery, List<ProjectDto>>
{
    private readonly IProjectRepository _repository = repository;
    private readonly ILogger<GetProjectsByOwnerIdHandler> _logger = logger;

    public async Task<List<ProjectDto>> Handle(GetProjectsByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        var projects = await _repository.GetAllByUserId(request.OwnerId);

        if (!projects.Any())
        {
            _logger.LogWarning("No projects found for owner {OwnerId}", request.OwnerId);
            return [];
        }

        _logger.LogInformation("{Count} projects retrieved for owner {OwnerId}",
            projects.Count, request.OwnerId);

        return projects.Adapt<List<ProjectDto>>();
    }
}

