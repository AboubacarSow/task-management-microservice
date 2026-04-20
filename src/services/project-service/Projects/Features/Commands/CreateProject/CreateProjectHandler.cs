<<<<<<< HEAD
namespace project_service.Projects.Features.Commands.CreateProject;

public record CreateProjectCommand(string Name,Guid CreatedByUser,string? Description) : IRequest<Guid>;
    
public class CreateProjectCommandValidator:AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty()
            .WithMessage("Name field is required");

        RuleFor(p => p.CreatedByUser).NotEmpty()
            .WithMessage("CreatedByUser field is required");
    }
}
public class CreateProjectHandler(IProjectRepository projectRepository,ILogger<CreateProjectHandler> logger) : IRequestHandler<CreateProjectCommand,Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating project {ProjectName} for user {UserId}",
            request.Name, request.CreatedByUser);
        var project = string.IsNullOrWhiteSpace(request.Description)
            ? new Project(request.Name,request.CreatedByUser)
            : new Project(request.Name,request.CreatedByUser,request.Description);

        await projectRepository.AddAsync(project);
        logger.LogInformation("Project with Id:{ProjectId} created successfully", project.Id);
        return project.Id;
    }

}
=======
namespace project_service.Projects.Features.Commands.CreateProject;

public record CreateProjectCommand(string Name,Guid CreatedByUser,string? Description) : IRequest<Guid>;
    
public class CreateProjectCommandValidator:AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty()
            .WithMessage("Name field is required");

        RuleFor(p => p.CreatedByUser).NotEmpty()
            .WithMessage("CreatedByUser field is required");
    }
}
public class CreateProjectHandler(IProjectRepository projectRepository,ILogger<CreateProjectHandler> logger) : IRequestHandler<CreateProjectCommand,Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating project {ProjectName} for user {UserId}",
            request.Name, request.CreatedByUser);
        var project = string.IsNullOrWhiteSpace(request.Description)
            ? new Project(request.Name,request.CreatedByUser)
            : new Project(request.Name,request.CreatedByUser,request.Description);

        await projectRepository.AddAsync(project);
        logger.LogInformation("Project with Id:{ProjectId} created successfully", project.Id);
        return project.Id;
    }

}
>>>>>>> 05b451b (new_update)
