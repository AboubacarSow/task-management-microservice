using Grpc.Core;
using project_grpc_server;

namespace project_service.Projects.Grpc.Server;    

public class ProjectsGrpcService (IProjectRepository projectRepository,
 ILogger<ProjectsGrpcService> logger) : ProjectInfo.ProjectInfoBase
{
    private readonly IProjectRepository _repository = projectRepository;
    private readonly ILogger<ProjectsGrpcService> _logger = logger;

    public override async Task<ProjectModel> GetProjectById(GetProjectRequest request,
     ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var project = await _repository.GetByIdAsync(projectId);
        if (project is null)
        {
                _logger.LogWarning("Project with id {ProjectId} not found", request.ProjectId);
                throw new RpcException(new Status(StatusCode.NotFound, $"Project with id {request.ProjectId} not found"));

        }
        
        var model = new ProjectModel
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Group = { project.Group.Select(g => g.ToString()) },
            PeopleWorking = { project.PeopleWorking.Select(p => p.ToString()) }
         };
        

        _logger.LogInformation("Project with id {ProjectId} retrieved successfully", request.ProjectId);
        return model;

        
    }
}

   

