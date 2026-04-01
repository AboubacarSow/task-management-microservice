using Duende.IdentityModel.Client;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using project_grpc_server;

namespace task_service.Tasks.Grpc.Client;

public class ProjectClient(
    ProjectInfo.ProjectInfoClient client,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache)
{
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
                ClientId = "task-service",
                ClientSecret = "secret",
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

        return await client.GetProjectByIdAsync(
            new GetProjectRequest { ProjectId = projectId }, headers);
    }
}