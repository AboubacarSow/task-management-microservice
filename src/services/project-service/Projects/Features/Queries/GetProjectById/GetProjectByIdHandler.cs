using FluentValidation;
using Mapster;
using MediatR;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Projects.Dtos;
using project_service.Projects.Models;

namespace project_service.Projects.Features.Queries.GetProjectById;


public record GetProjectByIdQuery(Guid Id):IRequest<ProjectDto>;
public class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery> 
{ 
    public GetProjectByIdQueryValidator()
    {
        RuleFor(p => p.Id).NotEmpty()
            .WithMessage("Id field is required");
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
        var project = await _repository.GetByIdAsync(request.Id);
        if (project == null) {
            _logger.LogWarning("Project with ID {ProjectId} was not found",
                request.Id);
            throw new NotFoundException(nameof(Project),request.Id.ToString());
        }

        _logger.LogInformation("Project with ID {ProjectId} retrieved successfully", project.Id);
        var dto = project.Adapt<ProjectDto>();
        return dto;
        //return new ProjectDto(
        //    project.Id,
        //    project.Name,
        //    project.CreatedAt,
        //    project.LastUpdatedAt,
        //    project.DueAt,
        //    project.Description,
        //    project.Status,
        //    project.CreatedByUser
        //);
    }
}
