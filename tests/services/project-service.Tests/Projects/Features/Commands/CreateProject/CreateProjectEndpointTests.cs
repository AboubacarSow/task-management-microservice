
using project_service.Projects.Features.Commands.CreateProject;

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

                services.AddAuthentication(options=>{
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme= "Test";
                }).AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

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
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test","fake");

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
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());
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


    [Fact]
    public async Task POST_Projects_Should_Return_401_When_UnauthenticatedAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;


        var request = new
        {
            Name = "Building a web scraping",
            Description = "Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);
        //Assert

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

    }

    [Fact]
    public async Task POST_Projects_Should_Return_Correct_Location_HeaderAsync()
    {
        var expectedId = Guid.NewGuid();
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var request = new
        {
            Name = "Building a web scraping",
            Description = "Building a web application using TDD approach"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/projects/{expectedId}");
    }
    [Fact]
    public async Task POST_Projects_Should_Return_400_When_Name_Is_MissingAsync()
    {
        var factory = _webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ISender>();
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblyContaining<CreateProjectCommand>();
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>),
                                    typeof(ValidationBehavior<,>));
                });
                services.AddValidatorsFromAssemblyContaining<CreateProjectCommandValidator>();
            });
        });
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
            var request = new
        {
            Name = string.Empty,
            Description = "Some description"
        };

        var response = await client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task POST_Projects_Should_Return_201_When_Description_Is_NullAsync()
    {
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var request = new
        {
            Name = "Building a web scraping",
            Description = (string?)null
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_Projects_Should_Send_Null_Description_When_Not_ProvidedAsync()
    {
        CreateProjectCommand? capturedCommand = null;
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cm, _) =>
            {
                capturedCommand = (CreateProjectCommand)cm;
            })
            .ReturnsAsync(Guid.NewGuid());

        var request = new
        {
            Name = "Building a web scraping",
            Description = (string?)null
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Description.Should().BeNull();
    }

    [Fact]
    public async Task POST_Projects_Should_Return_Body_With_Same_Id_As_HandlerAsync()
    {
        var expectedId = Guid.NewGuid();
        _senderMock.Setup(r => r.Send(It.IsAny<CreateProjectCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var request = new
        {
            Name = "Building a web scraping",
            Description = "Some description"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);
        var body = await response.Content.ReadFromJsonAsync<CreateProjectResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.Should().NotBeNull();
        body!.Id.Should().Be(expectedId); 
    }

}