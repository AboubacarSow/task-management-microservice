
namespace project_service.Tasks.Features.Commands.PauseTask;

public record PauseTaskRequest(string Note);
public class PauseTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id}/pause",
            async (Guid id, PauseTaskRequest request,
                ISender sender, IUserContext userContext) =>
            {
                await sender.Send(
                    new PauseTaskCommand(id, userContext.GetUserId(), request.Note)
                );

                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("PauseTask");
    }
}
