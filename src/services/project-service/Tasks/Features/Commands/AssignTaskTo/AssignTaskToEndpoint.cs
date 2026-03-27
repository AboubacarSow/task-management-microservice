


namespace project_service.Tasks.Features.Commands.AssignTaskTo;

public record AssignTaskToRequest(Guid UserId);
public class AssignTaskToEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{taskId:guid}/assign",
            async (
                Guid taskId,
                [FromBody]AssignTaskToRequest request,
                [FromServices]ISender sender,
                [FromServices]ClaimsPrincipal claims) =>
            {

                var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);


                var command = new AssignTaskToCommand(
                    taskId,
                    userId,
                    request.UserId);

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("AssignTask");
    }
}