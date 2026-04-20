using shared.Utilities;

namespace task_service.Tasks.Features.Commands.CompleteTask;
public record CompleteTaskRequest(string Notes);
public class CompleteTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/complete", async ([FromRoute]Guid id, 
        [FromBody]CompleteTaskRequest request, 
        [FromServices]ISender sender,
        [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();
            var command = new CompleteTaskCommand(id,userId,request.Notes);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CompleteTask");
    }
}