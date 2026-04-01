
namespace project_service.Tasks.Features.Commands.PauseTask;

public record PauseTaskRequest(string Note);
public class PauseTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id}/pause",
            async (Guid id, [FromBody]PauseTaskRequest request,
                [FromServices]ISender sender,
                [FromServices]ClaimsPrincipal claims) =>
            {
                var currentUserId =Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)?.Value!);

                await sender.Send(
                    new PauseTaskCommand(id, currentUserId, request.Note)
                );

                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("PauseTask");
    }
}
