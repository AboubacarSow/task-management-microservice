namespace task_service.Tasks.Features.Commands.UnAssignTask;

public class UnAssignTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        app.MapPatch("/api/tasks/{id:guid}/unassign",
            async (Guid id, [FromServices]ISender sender, 
            ClaimsPrincipal claims) =>
        {
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");

            await sender.Send(new UnAssignTaskCommand(id, userId));
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("UnassignTask");
    }

   
}