using FluentValidation;
using Mapster;
using MediatR;
using project_service.Data.Repositories;
using project_service.Projects.Dtos;
using System.Collections;

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
public class GetProjectsByOwnerIdHandler : IRequestHandler<GetProjectsByOwnerIdQuery, List<ProjectDto>>
{
    private readonly IProjectRepository _repository;
    private readonly ILogger<GetProjectsByOwnerIdHandler> _logger;
    public GetProjectsByOwnerIdHandler(IProjectRepository repository, ILogger<GetProjectsByOwnerIdHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

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

