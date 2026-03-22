using Carter;
using MediatR;
using project_service.Data.Utilities;

namespace project_service.Projects.Features.Commands.AddUserToGroup;


public record AddUserToGroupRequest(Guid UserId);
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

            var userId = userContext.GetUserId();

            var command = new AddUserToGroupCommand(request.UserId,id);

            await sender.Send(command);

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("AddCollaborator")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
