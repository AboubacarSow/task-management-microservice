<<<<<<< HEAD
﻿
namespace project_service.Tasks.Features.Commands.PauseTask;

public record PauseTaskRequest(string Note);
public class PauseTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id}/pause",
            async (Guid id, [FromBody]PauseTaskRequest request,
                [FromServices]ISender sender,
                ClaimsPrincipal claims) =>
            {
                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(currentUserId, out var userId))
                    return Results.BadRequest("Invalid user id");
                    
                await sender.Send(
                    new PauseTaskCommand(id, userId, request.Note)
                );

                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("PauseTask");
    }
}
=======
﻿
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
>>>>>>> 05b451b (new_update)
