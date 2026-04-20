
using shared.Utilities;

namespace task_service.Tasks.Features.Commands.PauseTask;

public record PauseTaskRequest(string Note);
public class PauseTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id}/pause",
            async (Guid id, [FromBody]PauseTaskRequest request,
                [FromServices]ISender sender,
                [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();

                await sender.Send(
                    new PauseTaskCommand(id, userId, request.Note)
                );

                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("PauseTask");
    }
}
