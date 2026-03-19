using Mapster;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using project_service.Data.Repositories;
using project_service.Projects.Models;

namespace project_service.Projects.Features.Commands.CreateProject;

public record CreateProjectCommand(string Name,Guid CreatedByUser,string? Description) : IRequest<Guid>;
    

public class CreateProjectHandler(IProjectRepository projectRepository) : IRequestHandler<CreateProjectCommand,Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var project = string.IsNullOrWhiteSpace(request.Description)
            ? new Project(request.Name,request.CreatedByUser)
            : new Project(request.Name,request.CreatedByUser,request.Description);

        await projectRepository.AddAsync(project);

        return project.Id;
    }

}