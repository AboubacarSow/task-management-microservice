namespace project_service.Tasks.Features.Commands.EditTask;

public record EditTaskRequest(string Title,DateTime DueAt,string Description);
public class EditTaskEndpoint : ICarterModule{

    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPatch("/api/tasks/{taskId:guid}",
            async (
                Guid taskId,
                EditTaskRequest req,
                ISender sender,
                IUserContext userContext) =>
        {
            var currentUserId = userContext.GetUserId();
            var result = await sender.Send(new EditTaskCommand(
                currentUserId,
                taskId,
                req.Title,
                req.DueAt,
                req.Description));

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("UpdateTask");

    }
}