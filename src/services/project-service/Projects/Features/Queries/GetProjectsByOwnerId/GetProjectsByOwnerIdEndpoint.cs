using shared.Utilities;

namespace project_service.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/me", async (
            [FromServices]ISender sender,
            [FromServices] IUserContext userContext) =>
        {
           var userId = userContext.GetUserId();
           var query = new GetProjectsByOwnerIdQuery(userId);
            var result = await sender.Send(query);
            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetAllByOwner");
        
    }
}

