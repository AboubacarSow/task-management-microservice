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
}