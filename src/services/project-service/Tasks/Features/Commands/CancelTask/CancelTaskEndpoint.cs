namespace project_service.Tasks.Features.Commands.CancelTask;


public class CancelTaskEnpoint :ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/cancel", async (Guid id,
            ISender sender , IUserContext userContext)=>
           {
            var currentUserId = userContext.GetUserId();
            var command = new CancelTaskCommand(currentUserId,id);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CancelTask");
    }
}