namespace task_service.Tasks.Features.Commands.CancelTask;


public class CancelTaskEnpoint :ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/cancel", async ([FromRoute]Guid id,
            [FromServices]ISender sender , ClaimsPrincipal claims)=>
           {
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");  
            var command = new CancelTaskCommand(userId,id);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CancelTask");
    }
}