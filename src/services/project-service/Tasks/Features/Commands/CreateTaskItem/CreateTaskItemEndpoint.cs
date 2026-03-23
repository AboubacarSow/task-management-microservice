


namespace project_service.Tasks.Features.Commands.CreateTaskItem;

public record CreateTaskItemRequest(Guid ProjectId, string Title);
public class CreateTaskItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks",
            async (
                CreateTaskItemRequest request,
                ISender sender,
                IUserContext userContext) =>
            {

                var userId = userContext.GetUserId();
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
