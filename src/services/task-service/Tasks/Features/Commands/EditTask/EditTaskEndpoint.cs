using shared.Utilities;

namespace task_service.Tasks.Features.Commands.EditTask;

public record EditTaskRequest(string Title,DateTime DueAt,string Description);
public class EditTaskEndpoint : ICarterModule{

    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPatch("/api/tasks/{taskId:guid}",
            async (
                Guid taskId,
                [FromBody]EditTaskRequest req,
                [FromServices]ISender sender,
                 [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();

                var result = await sender.Send(new EditTaskCommand(
                userId,
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