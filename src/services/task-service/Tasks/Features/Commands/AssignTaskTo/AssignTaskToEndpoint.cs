namespace task_service.Tasks.Features.Commands.AssignTaskTo;

public record AssignTaskToRequest(Guid UserId);
public class AssignTaskToEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{taskId:guid}/assign",
            async (
                Guid taskId,
                [FromBody]AssignTaskToRequest request,
                [FromServices]ISender sender,
                ClaimsPrincipal claims
                ) =>
            {

                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");
                //var userId = Guid.NewGuid();
                var command = new AssignTaskToCommand(
                    taskId,
                    userId,
                    request.UserId);

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("AssignTask");
    }
}