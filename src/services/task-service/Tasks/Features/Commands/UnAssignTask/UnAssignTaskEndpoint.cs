using shared.Utilities;

namespace task_service.Tasks.Features.Commands.UnAssignTask;

public class UnAssignTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        app.MapPatch("/api/tasks/{id:guid}/unassign",
            async (Guid id, [FromServices]ISender sender,
            [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();

                await sender.Send(new UnAssignTaskCommand(id, userId));
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("UnassignTask");
    }

   
}