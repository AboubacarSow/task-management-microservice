


namespace project_service.Tasks.Features.Commands.CreateTaskItem;

public record CreateTaskItemRequest(Guid ProjectId, string Title);
public class CreateTaskItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks",
            async (
                [FromBody]CreateTaskItemRequest request,
                [FromServices]ISender sender,
                ClaimsPrincipal claims
                ) =>
            {

                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(currentUserId, out var userId))
                    return Results.BadRequest("Invalid user id");


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
