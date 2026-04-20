using MongoDB.Bson;
using task_service.Tasks.Features.Commands.PauseTask;
using task_service.Tests.Fixtures;

namespace task_service.Tests.Tasks.Features.Commands.PauseTask
{
    public class PauseTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<ISender> _senderMock = new();

        private readonly Guid _taskId = Guid.NewGuid();
        private  readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public PauseTaskEndpointTests(WebApplicationFactory<Program> factory)
        {
            var _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(_senderMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                    }).AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

                    services.AddAuthorization();
                    services.AddHttpContextAccessor();
                    services.AddScoped<IUserContext, FakeUserContext>();
                });
            });

            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "fake");
        }

        [Fact]
        public async Task PATCH_PauseTask_ShouldReturn204()
        {
            _senderMock.Setup(s => s.Send(It.IsAny<PauseTaskCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Unit.Value);
            var request = new {
                note = "random note"
            };

            var response = await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}/pause", request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PATCH_PauseTask_ShouldSendCorrectCommand()
        {
            PauseTaskCommand? captured = null;

            _senderMock
                .Setup(s => s.Send(It.IsAny<PauseTaskCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                {
                    captured = (PauseTaskCommand)cmd;
                })
                .ReturnsAsync(Unit.Value);

            var request = new PauseTaskRequest("Focus break");

            await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}/pause", request);

            captured.Should().NotBeNull();
            captured!.TaskId.Should().Be(_taskId);
            captured.UserId.Should().Be(_userId);
            captured.Notes.Should().Be("Focus break");
        }

        [Fact]
        public async Task PATCH_PauseTask_ShouldReturn401_WhenUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PatchAsync($"/api/tasks/{_taskId}/pause", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PATCH_PauseTask_ShouldHandleEmptyNote()
        {
            PauseTaskCommand? captured = null;

            _senderMock
                .Setup(s => s.Send(It.IsAny<PauseTaskCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Unit>, CancellationToken>((cmd, _) =>
                {
                    captured = (PauseTaskCommand)cmd;
                })
                .ReturnsAsync(Unit.Value);

            var request = new PauseTaskRequest(string.Empty);

            var response = await _client.PatchAsJsonAsync($"/api/tasks/{_taskId}/pause", request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            captured!.Notes.Should().BeEmpty();
        }
    }
}