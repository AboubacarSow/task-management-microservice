using shared.Utilities;

namespace task_service.Tasks.Features.Commands.CancelTask;


public class CancelTaskEnpoint :ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/cancel", async ([FromRoute]Guid id,
            [FromServices]ISender sender,
            [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();
            var command = new CancelTaskCommand(userId,id);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CancelTask");
    }
}