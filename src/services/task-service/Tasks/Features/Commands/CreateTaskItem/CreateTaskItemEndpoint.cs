using shared.Utilities;

namespace task_service.Tasks.Features.Commands.CreateTaskItem;

public record CreateTaskItemRequest(Guid ProjectId, string Title);
public class CreateTaskItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks",
            async (
                [FromBody]CreateTaskItemRequest request,
                [FromServices]ISender sender,
                 [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();


                //var userId = Guid.NewGuid();
                var command = new CreateTaskItemCommand(
                    userId,
                    request.ProjectId,
                    request.Title);

                var taskId = await sender.Send(command);


                return Results.Created($"/api/tasks/{taskId}", new { id = taskId });
            })
        .RequireAuthorization()
        .WithName("CreateTask");
    }
}
