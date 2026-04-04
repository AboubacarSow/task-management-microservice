using Duende.IdentityModel.Client;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace shared.Interceptors;

public class ServiceTokenInterceptor(IMemoryCache cache,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return new AsyncUnaryCall<TResponse>(
            HandleAsync(request, context, continuation),
            null!, null!, null!, null!);
    }

    private async Task<TResponse> HandleAsync<TRequest, TResponse>(
    TRequest request,
    ClientInterceptorContext<TRequest, TResponse> context,
    AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    where TRequest : class
    where TResponse : class
    {
        var token = await GetTokenAsync();

        Console.WriteLine($"FULL TOKEN: '{token}'");
        Console.WriteLine($"TOKEN PARTS: {token?.Split('.').Length}");
        
        var headers = new Metadata(); 

        if (context.Options.Headers != null)
        {
            foreach (var header in context.Options.Headers)
            {
                if (!header.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
                    headers.Add(header);
            }
        }

        headers.Add("Authorization", $"Bearer {token}");
        Console.WriteLine("headers: " + string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}")));
        var newOptions = context.Options.WithHeaders(headers);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            newOptions);

        return await continuation(request, newContext).ResponseAsync;
    }

    private async Task<string> GetTokenAsync()
    {
        if (cache.TryGetValue("grpc_service_token", out string? cached))
        {
            Console.WriteLine("Using cached token: " + cached );
            return cached!;
        }

        var httpClient = httpClientFactory.CreateClient();
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
                Address      = disco.TokenEndpoint,
                ClientId     = "task-service",
                ClientSecret = "task-secret",
                Scope        = "project_read"
            });

        if (tokenResponse.IsError)
            throw new Exception($"Service token request failed: {tokenResponse.Error}");

        cache.Set("grpc_service_token", tokenResponse.AccessToken,
            TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));

        return tokenResponse.AccessToken!;
    }
}