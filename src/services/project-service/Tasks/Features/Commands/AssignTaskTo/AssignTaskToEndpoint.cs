


namespace project_service.Tasks.Features.Commands.AssignTaskTo;

public record AssignTaskToRequest(Guid UserId);
public class AssignTaskToEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{taskId:guid}/assign",
            async (
                Guid taskId,
                AssignTaskToRequest request,
                ISender sender,
                IUserContext userContext) =>
            {

                var currentUserId = userContext.GetUserId();


                var command = new AssignTaskToCommand(
                    taskId,
                    currentUserId,
                    request.UserId);

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("AssignTask");
    }
}