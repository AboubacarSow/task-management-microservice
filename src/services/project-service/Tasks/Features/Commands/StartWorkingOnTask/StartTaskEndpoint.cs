namespace project_service.Tasks.Features.Commands.StartWorkingOnTask;

public class StartTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks/{id:guid}/start",
            async (Guid id, ISender sender, IUserContext userContext) =>
            {
                var command = new StartTaskCommand(id, userContext.GetUserId());
                await sender.Send(command);
                return Results.NoContent();
            })
        .RequireAuthorization()
        .WithName("StartTask");
    }
}