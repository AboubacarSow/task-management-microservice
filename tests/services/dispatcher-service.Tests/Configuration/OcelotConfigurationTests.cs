using dispatcher_service.Tests.Fixures;
using Microsoft.Extensions.Configuration;

namespace dispatcher_service.Tests.Configuration;

public class OcelotConfigurationTests
{
    private readonly IConfiguration _configuration;
    private readonly List<OcelotRoute> _routes;

    public OcelotConfigurationTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("ocelot.json", optional: false)
            .Build();

        _routes = _configuration
            .GetSection("Routes")
            .Get<List<OcelotRoute>>()!;
    }

    [Fact]
    public void OcelotConfig_RoutesSection_IsNotEmpty()
    {
        _routes.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void OcelotConfig_AllRoutes_HaveDownstreamPathTemplate()
    {
        _routes.Should().AllSatisfy(r =>
            r.DownstreamPathTemplate.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void OcelotConfig_AllRoutes_HaveDownstreamHostAndPorts()
    {
        _routes.Should().AllSatisfy(r =>
            r.DownstreamHostAndPorts.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void OcelotConfig_AllRoutes_UseHttpScheme()
    {
        _routes.Should().AllSatisfy(r =>
            r.DownstreamScheme.Should().Be("http"));
    }

    [Fact]
    public void OcelotConfig_AllRoutes_HaveUpstreamHttpMethods()
    {
        _routes.Should().AllSatisfy(r =>
            r.UpstreamHttpMethod.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void OcelotConfig_AuthenticatedRoutes_HaveBearerProviderKey()
    {
        var authenticatedRoutes = _routes
            .Where(r => r.AuthenticationOptions is not null);

        authenticatedRoutes.Should().AllSatisfy(r =>
            r.AuthenticationOptions!.AuthenticationProviderKey
                .Should().Be("Bearer"));
    }

    [Fact]
    public void OcelotConfig_AuthenticatedRoutes_HaveAllowedScopes()
    {
        var authenticatedRoutes = _routes
            .Where(r => r.AuthenticationOptions is not null);

        authenticatedRoutes.Should().AllSatisfy(r =>
            r.AuthenticationOptions!.AllowedScopes.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void OcelotConfig_LoginRoute_IsPublic()
    {
        var loginRoute = _routes
            .FirstOrDefault(r => r.UpstreamPathTemplate == "/login");

        loginRoute.Should().NotBeNull();
        loginRoute!.AuthenticationOptions.Should().BeNull();
    }

    [Fact]
    public void OcelotConfig_UserRegistrationRoute_IsPublic()
    {
        var registrationRoute = _routes
            .FirstOrDefault(r => r.UpstreamPathTemplate == "/users/");

        registrationRoute.Should().NotBeNull();
        registrationRoute!.AuthenticationOptions.Should().BeNull();
    }

    [Fact]
    public void OcelotConfig_AllRoutes_DownstreamHostsAreNotEmpty()
    {
        _routes.Should().AllSatisfy(r =>
            r.DownstreamHostAndPorts.Should().AllSatisfy(h =>
                h.Host.Should().NotBeNullOrEmpty()));
    }

    [Fact]
    public void Debug_PrintAllUpstreamPaths()
    {
        var paths = _routes.Select(r => r.UpstreamPathTemplate).ToList();
        Console.Write(paths);
        paths.Should().NotBeEmpty();
        // This will fail and print all paths in the error message
    }
}