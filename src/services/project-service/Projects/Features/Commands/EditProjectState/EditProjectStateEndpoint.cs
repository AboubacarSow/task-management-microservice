namespace project_service.Projects.Features.Commands.EditProjectState;


public record EditProjectStateRequest(ProjectStatus Status);

public class EditProjectStateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/projects/{id:guid}/state", async (
                Guid id,
                [FromBody]EditProjectStateRequest request,
                [FromServices]ISender sender,
                [FromServices]IUserContext userContext) =>
        {
            var command = new EditProjectStateCommand(
                id,
                userContext.GetUserId(),
                request.Status
            );

            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
            .WithName("UpdateStatus")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)   // caught by pipeline
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}