using shared.Utilities;

namespace project_service.Projects.Features.Commands.AddUserToGroup;


public record AddUserToGroupRequest(Guid TargetUserId);
public class AddUserToGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{id:guid}/group",async (
            Guid id,
            [FromBody]AddUserToGroupRequest request,
            [FromServices]ISender sender,
            [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();

            var command = new AddUserToGroupCommand(request.TargetUserId,id,userId);

            await sender.Send(command);

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("AddCollaborator")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
