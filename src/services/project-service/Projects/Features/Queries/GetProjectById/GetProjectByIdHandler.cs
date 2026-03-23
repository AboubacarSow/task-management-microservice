using FluentValidation;
using Mapster;
using MediatR;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Projects.Dtos;
using project_service.Projects.Models;

namespace project_service.Projects.Features.Queries.GetProjectById;


public record GetProjectByIdQuery(Guid CurrentUserId,Guid ProjectId):IRequest<ProjectDto>;
public class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery> 
{ 
    public GetProjectByIdQueryValidator()
    {
        RuleFor(p => p.ProjectId).NotEmpty()
            .WithMessage("ProjectId field is required");

        RuleFor(p => p.CurrentUserId).NotEmpty()
            .WithMessage("CurrentUserId field is required");
    }
}

public class GetProjectByIdHandler:IRequestHandler<GetProjectByIdQuery,ProjectDto>
{
    private readonly IProjectRepository _repository;
    private readonly ILogger<GetProjectByIdHandler> _logger;
    public GetProjectByIdHandler(IProjectRepository repository, ILogger<GetProjectByIdHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(request.ProjectId);

        if (project == null) {
            _logger.LogWarning("Project with ID {ProjectId} was not found",
                request.ProjectId);
            throw new NotFoundException(nameof(Project),request.ProjectId.ToString());
        }

        if (!project.IsInPeopleWorking(request.CurrentUserId))
        {
            _logger.LogWarning("UNAUTHORIZED_USER to read Project with {ProjectId}",
                request.ProjectId);
            throw new ForbiddenException(request.CurrentUserId.ToString(), "READ_PROJECT");
        }

         _logger.LogInformation("Project with ID {ProjectId} retrieved successfully", project.Id);
        var dto = project.Adapt<ProjectDto>();
        return dto;
    }
}
