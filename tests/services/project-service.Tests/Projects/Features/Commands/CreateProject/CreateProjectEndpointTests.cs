using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Projects.Features.Commands.CreateProject;
using project_service.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace project_service.Tests.Projects.Features.Commands.CreateProject;

public class CreateProjectEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly Mock<ISender> _senderMock = new();
    public CreateProjectEndpointTests(WebApplicationFactory<Program> factory)
    {
        _webApplicationFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(_senderMock.Object);

                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                services.AddAuthorization();

                services.AddHttpContextAccessor();
                services.AddScoped<IUserContext, HttpUserContext>();
            });
        });
        _client = _webApplicationFactory.CreateClient
            (new WebApplicationFactoryClientOptions  
            {
                AllowAutoRedirect =false
            });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

    }

    [Fact]
    public async Task POSTProjects_ShouldReturn201()
    {

        var request = new 
        {
            Name="Building a web scraping",
            Description="Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        //Assert

        response.StatusCode.Should().Be(HttpStatusCode.Created);
      
    }

    [Fact]
    public async Task POSTProjects_ShouldReturn201AndResponseBodyAsync()
    {
        var request = new 
        {
            Name="Building a web scraping",
            Description="Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects",request);
        //Assert

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreateProjectResponse>();

        body.Should().NotBeNull();
        body.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task POST_Projects_Should_Use_MediatR_HandlerAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var request = new
        {
            Name = "Building a web scraping",
            Description = "Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);
        //Assert

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _senderMock.Verify(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task POST_Projects_Should_Send_Correct_CommandAsync()
    {
        CreateProjectCommand? capturedCommand = null ;
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
           It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken> ((cm, _) =>
            {
                capturedCommand =(CreateProjectCommand) cm;
            })
           .ReturnsAsync(Guid.NewGuid());


        var request = new
        {
            Name = "Building a web scraping",
            Description = "Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);
        //Assert

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        capturedCommand.Should().NotBeNull();
        capturedCommand.Name.Should().Be(request.Name);
        capturedCommand.Description.Should().Be(request.Description);
        capturedCommand.CreatedByUser.Should()
        .Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }


}