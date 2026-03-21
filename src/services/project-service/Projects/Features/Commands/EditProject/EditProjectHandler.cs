using FluentValidation;
using MediatR;
using project_service.Commons.Exceptions;
using project_service.Data.Repositories;
using project_service.Projects.Models;

namespace project_service.Projects.Features.Commands.EditProject;


public record EditProjectCommand(Guid ProjectId,
     Guid UserId, 
     string? Description, DateTime? DueAt): IRequest;



public class EditProjectCommandValidator : AbstractValidator<EditProjectCommand>
{
    public EditProjectCommandValidator(){
        RuleFor(p=>p.ProjectId)
        .NotEmpty()
        .WithMessage("ProjectId is required");

        RuleFor(p=>p.UserId).NotEmpty()
        .WithMessage("UserId is required");
    }
}
public class EditProjectHandler(IProjectRepository repository,ILogger<EditProjectHandler> logger)
: IRequestHandler<EditProjectCommand>
{
   private readonly IProjectRepository _repository = repository;
   private readonly ILogger<EditProjectHandler> _logger = logger;

    public async Task Handle(EditProjectCommand request, CancellationToken none)
    {
        _logger.LogInformation("Editing Project {ProjectId} by User {UserId}",
        request.ProjectId,
        request.UserId);
        
        var project = await _repository.GetByIdAsync(request.ProjectId)
        ?? throw new NotFoundException(nameof(Project), request.ProjectId.ToString());

        if(project.CreatedByUser != request.UserId)
            throw new ForbiddenException(request.UserId.ToString(), "EDIT_PROJECT");


        if(request.Description is not null)
            project.SetDescription(request.Description);

        if(request.DueAt is not null)
            project.SetDueDate(request.DueAt.Value);
        
        await _repository.EditAsync(project);
        _logger.LogInformation("UPDATE_SUCCESSFULLY Project {ProjectId} ",request.ProjectId);

    }
}


