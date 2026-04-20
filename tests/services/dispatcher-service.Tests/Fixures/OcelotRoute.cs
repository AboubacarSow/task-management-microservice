namespace dispatcher_service.Tests.Fixures;

public  class OcelotRoute
{
    public string DownstreamPathTemplate { get; set; } = string.Empty;
    public string UpstreamPathTemplate { get; set; } = string.Empty;
    public string DownstreamScheme { get; set; } = string.Empty;
    public List<DownstreamHost> DownstreamHostAndPorts { get; set; } = new();
    public List<string> UpstreamHttpMethod { get; set; } = new();
    public AuthOptions? AuthenticationOptions { get; set; }
}

public class DownstreamHost
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}

public class AuthOptions
{
    public string AuthenticationProviderKey { get; set; } = string.Empty;
    public List<string> AllowedScopes { get; set; } = new();
}