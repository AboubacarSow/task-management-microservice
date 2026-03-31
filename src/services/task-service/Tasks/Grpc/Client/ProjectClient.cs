using project_grpc_server;

namespace task_service.Tasks.Grpc.Client;

public class ProjectClient(ProjectInfo.ProjectInfoClient client)
{
    private readonly ProjectInfo.ProjectInfoClient _client = client;

 

    public async Task<ProjectModel> GetProjectAsync(string projectId)
        => await _client.GetProjectByIdAsync(
            new GetProjectRequest { 
                ProjectId = projectId });
}