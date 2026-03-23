

namespace project_service.Projects.Features.Commands.AddUserToGroup;


public record AddUserToGroupRequest(Guid TargetUserId);
public class AddUserToGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{id:guid}/group",async (
            Guid id,
            AddUserToGroupRequest request,
            ISender sender,
            IUserContext userContext
            ) =>
        {
            // Will be used later on to check if user is authorize to perform such operation
            var userId = userContext.GetUserId();

            var command = new AddUserToGroupCommand(request.TargetUserId,id);

            await sender.Send(command);

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("AddCollaborator")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
