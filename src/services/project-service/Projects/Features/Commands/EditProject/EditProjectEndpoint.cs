namespace project_service.Projects.Features.Commands.EditProject;
public record EditProjectRequest(string? Description, DateTime? DueAt);

public class EditProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/projects/{id:guid}", async (
            Guid id,
            [FromBody]EditProjectRequest request,
            [FromServices]ISender sender,
            [FromServices]IUserContext userContext) =>
        {
            var command = new EditProjectCommand(
                id,
                userContext.GetUserId(),
                request.Description,
                request.DueAt
            );

            await sender.Send(command);

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("EditProject")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}

