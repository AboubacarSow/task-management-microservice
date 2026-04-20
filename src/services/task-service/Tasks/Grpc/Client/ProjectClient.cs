using Duende.IdentityModel.Client;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace task_service.Tasks.Grpc.Client;


public interface IProjectClient 
{
    public Task<ProjectModel> GetProjectAsync(string projectId);
}
public class ProjectClient(
    ProjectInfo.ProjectInfoClient client,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache) :IProjectClient
{
    private async Task<string> GetTokenAsync()
    {
        if (cache.TryGetValue("grpc_access_token", out string? cached))
            return cached!;

        var httpClient = httpClientFactory.CreateClient();
        Console.WriteLine("AUTHORITY: " + configuration["IdentityServer:Authority"]);
        var disco = await httpClient.GetDiscoveryDocumentAsync(
            new DiscoveryDocumentRequest
            {
                Address = configuration["IdentityServer:Authority"],
                Policy =
                {
                    RequireHttps = false
                }
            });

        if (disco.IsError)
            throw new Exception($"Discovery document error: {disco.Error}");

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "task-service",
                ClientSecret = "task-secret",
                Scope = "project_read"
            });

        if (tokenResponse.IsError)
            throw new Exception($"Token request failed: {tokenResponse.Error}");

        cache.Set("grpc_access_token", tokenResponse.AccessToken,
            TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));

        return tokenResponse.AccessToken!;
    }

    public async Task<ProjectModel> GetProjectAsync(string projectId)
    {
        var token = await GetTokenAsync();
        var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
        Console.WriteLine("headers: " + string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}")));
        Console.WriteLine($"Requesting project {projectId} with token: {token}");
        return await client.GetProjectByIdAsync(
            new GetProjectRequest { ProjectId = projectId }, headers);
    }
}