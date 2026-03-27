namespace project_service.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks/{id:guid}/start",
            async (Guid id, [FromServices]ISender sender, ClaimsPrincipal claims) =>
            {

                var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
                var command = new StartTaskCommand(id, userId);
                await sender.Send(command);
                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("StartTask");
    }
}