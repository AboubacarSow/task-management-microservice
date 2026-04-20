<<<<<<< HEAD
using Duende.IdentityModel.Client;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using task_grpc_server;

namespace project_service.Projects.Grpc.Client;

public class TaskItemClient(TaskInfo.TaskInfoClient client,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache)
{
    
    private readonly TaskInfo.TaskInfoClient _client = client ;

    public async Task<TaskModel> GetTasksStatusAsync(string projectId)
    {
        var token = await GetTokenAsync();
        var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
        return  await _client.GetTaskByIdAsync(
            new GetTaskRequest 
            { 
                ProjectId = projectId
            },headers);
    }

    
    private async Task<string> GetTokenAsync()
    {
        if (cache.TryGetValue("grpc_access_token", out string? cached))
            return cached!;

        var httpClient = httpClientFactory.CreateClient();
        var disco = await httpClient.GetDiscoveryDocumentAsync(
            configuration["IdentityServer:Authority"]);

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "project-service",
                ClientSecret = "project-secret",
                Scope = "task_fullpermission"
            });

        if (tokenResponse.IsError)
            throw new Exception($"Token request failed: {tokenResponse.Error}");

        cache.Set("grpc_access_token", tokenResponse.AccessToken,
            TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));

        return tokenResponse.AccessToken!;
    }
=======
using Duende.IdentityModel.Client;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using task_grpc_server;

namespace project_service.Projects.Grpc.Client;

public interface ITaskItemClient
{
    public Task<TaskModel> GetTasksStatusAsync(string projectId);
}
public class TaskItemClient(TaskInfo.TaskInfoClient client,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache) :ITaskItemClient
{
    
    private readonly TaskInfo.TaskInfoClient _client = client ;

    public async Task<TaskModel> GetTasksStatusAsync(string projectId)
    {
        var token = await GetTokenAsync();
        var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
        return  await _client.GetTaskByIdAsync(
            new GetTaskRequest 
            { 
                ProjectId = projectId
            },headers);
    }

    
    private async Task<string> GetTokenAsync()
    {
        if (cache.TryGetValue("grpc_access_token", out string? cached))
            return cached!;

        var httpClient = httpClientFactory.CreateClient();
        var disco = await httpClient.GetDiscoveryDocumentAsync(
            configuration["IdentityServer:Authority"]);

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "project-service",
                ClientSecret = "project-secret",
                Scope = "task_fullpermission"
            });

        if (tokenResponse.IsError)
            throw new Exception($"Token request failed: {tokenResponse.Error}");

        cache.Set("grpc_access_token", tokenResponse.AccessToken,
            TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));

        return tokenResponse.AccessToken!;
    }
>>>>>>> 05b451b (new_update)
}