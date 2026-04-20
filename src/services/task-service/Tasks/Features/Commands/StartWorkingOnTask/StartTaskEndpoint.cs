<<<<<<< HEAD

namespace task_service.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks/{id:guid}/start",
            async (Guid id, [FromServices]ISender sender, 
            ClaimsPrincipal claims) =>
            {

                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(currentUserId, out var userId))
                    return Results.BadRequest("Invalid user id");


                var command = new StartTaskCommand(id, userId);
                await sender.Send(command);
                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("StartTask");
    }
=======

using shared.Utilities;

namespace task_service.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks/{id:guid}/start",
            async (Guid id, [FromServices]ISender sender,
               [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();


                var command = new StartTaskCommand(id, userId);
                await sender.Send(command);
                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("StartTask");
    }
>>>>>>> 05b451b (new_update)
}