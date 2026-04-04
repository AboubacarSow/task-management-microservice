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
            ClaimsPrincipal claims
            ) =>
        {
            // Will be used later on to check if user is authorize to perform such operation

            if(request.TargetUserId == Guid.Empty)
                return Results.BadRequest("Target user id is required");
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id"); 

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
