<<<<<<< HEAD
namespace project_service.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/group",
            async ([FromRoute]Guid projectId, [FromServices]ISender sender, 
            ClaimsPrincipal claims) =>
            {
                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");

                var result = await sender.Send(new GetGroupByIdQuery(userId,projectId));


                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetGroup");
    }
=======
using shared.Utilities;

namespace project_service.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/group",
            async ([FromRoute]Guid projectId, [FromServices]ISender sender,
            [FromServices] IUserContext claims) =>
            {
                var userId = claims.GetUserId();

                var result = await sender.Send(new GetGroupByIdQuery(userId,projectId));


                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetGroup");
    }
>>>>>>> 05b451b (new_update)
}