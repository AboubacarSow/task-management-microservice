using MediatR;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Projects.Models;

namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;


public record GetPeopleWorkingByIdQuery(Guid UserId,Guid ProjectId): IRequest<List<Guid>>;
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