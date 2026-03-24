namespace project_service.Tasks.Features.Commands.CompleteTask;
public record CompleteTaskRequest(string Notes);
public class CompleteTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/complete", async (Guid id, 
        CompleteTaskRequest request, 
        ISender sender, 
        IUserContext userContext)=>
        {
            var currentUserId = userContext.GetUserId();
            var command = new CompleteTaskCommand(id,currentUserId,request.Notes);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CompleteTask");
    }
}