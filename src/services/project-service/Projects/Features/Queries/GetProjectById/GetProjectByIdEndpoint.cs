<<<<<<< HEAD
namespace project_service.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{id:guid}", 
                async ([FromRoute]Guid id, [FromServices]ISender sender,
                ClaimsPrincipal claims) =>
        {
             var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");
            var query = new GetProjectByIdQuery(userId,id);
            var result = await sender.Send(query);

            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetById");
    }
}
=======
using shared.Utilities;

namespace project_service.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{id:guid}", 
                async ([FromRoute]Guid id, [FromServices]ISender sender,
                [FromServices] IUserContext claims) =>
        {
             var userId =claims.GetUserId();
            var query = new GetProjectByIdQuery(userId,id);
            var result = await sender.Send(query);

            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetById");
    }
}
>>>>>>> 05b451b (new_update)
