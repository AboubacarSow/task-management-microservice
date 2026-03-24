
namespace project_service.Tasks.Features.Commands.UnAssignTask;

public class UnAssignTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        app.MapPatch("/api/tasks/{id:guid}/unassign",
            async (Guid id, ISender sender, IUserContext userContext) =>
        {
            var current_userId = userContext.GetUserId();
            await sender.Send(new UnAssignTaskCommand(id, current_userId));
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("UnassignTask");
    }

   
}