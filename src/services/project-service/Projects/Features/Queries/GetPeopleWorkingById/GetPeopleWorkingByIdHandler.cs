<<<<<<< HEAD
namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;


public record GetPeopleWorkingByIdQuery(Guid UserId,Guid ProjectId): IRequest<List<Guid>>;

public class GetPeopleWorkingByIdQueryValidator: AbstractValidator<GetPeopleWorkingByIdQuery>{

    public GetPeopleWorkingByIdQueryValidator()
    {
        RuleFor(q=>q.UserId).NotEmpty().WithMessage("UserId is required");

        RuleFor(q=>q.ProjectId).NotEmpty().WithMessage("ProjectId is required");
    }
}
public class GetPeopleWorkingByIdHandler(IProjectRepository repository, ILogger<GetPeopleWorkingByIdHandler> logger)
        : IRequestHandler<GetPeopleWorkingByIdQuery, List<Guid>>
{
    private readonly IProjectRepository _repository = repository;
    private readonly ILogger<GetPeopleWorkingByIdHandler> _logger = logger;

    public async Task<List<Guid>> Handle(GetPeopleWorkingByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} is requesting PeopleWorking for Project {ProjectId}",
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

        if (!project.IsInGroup(request.UserId))
        {
            _logger.LogWarning(
                "Unauthorized access to PeopleWorking. User {UserId} is not in Group for Project {ProjectId}",
                request.UserId,
                request.ProjectId);

            throw new UnauthorizedAccessException();
        }

        _logger.LogInformation(
            "PeopleWorking retrieved successfully for Project {ProjectId} by User {UserId}",
            request.ProjectId,
            request.UserId);


        return [..project.PeopleWorking];
    }
}
=======
namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;


public record GetPeopleWorkingByIdQuery(Guid UserId,Guid ProjectId): IRequest<List<Guid>>;

public class GetPeopleWorkingByIdQueryValidator: AbstractValidator<GetPeopleWorkingByIdQuery>{

    public GetPeopleWorkingByIdQueryValidator()
    {
        RuleFor(q=>q.UserId).NotEmpty().WithMessage("UserId is required");

        RuleFor(q=>q.ProjectId).NotEmpty().WithMessage("ProjectId is required");
    }
}
public class GetPeopleWorkingByIdHandler(IProjectRepository repository, ILogger<GetPeopleWorkingByIdHandler> logger)
        : IRequestHandler<GetPeopleWorkingByIdQuery, List<Guid>>
{
    private readonly IProjectRepository _repository = repository;
    private readonly ILogger<GetPeopleWorkingByIdHandler> _logger = logger;

    public async Task<List<Guid>> Handle(GetPeopleWorkingByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} is requesting PeopleWorking for Project {ProjectId}",
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

        if (!project.IsInGroup(request.UserId))
        {
            _logger.LogWarning(
                "Unauthorized access to PeopleWorking. User {UserId} is not in Group for Project {ProjectId}",
                request.UserId,
                request.ProjectId);

            throw new UnauthorizedAccessException();
        }

        _logger.LogInformation(
            "PeopleWorking retrieved successfully for Project {ProjectId} by User {UserId}",
            request.ProjectId,
            request.UserId);


        return [..project.PeopleWorking];
    }
}
>>>>>>> 05b451b (new_update)
