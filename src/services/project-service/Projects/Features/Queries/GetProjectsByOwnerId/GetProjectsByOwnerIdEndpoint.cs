using Carter;
using MediatR;
using project_service.Data.Utilities;

namespace project_service.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/me", async (
            ISender sender,
            IUserContext userContext) =>
        {
            var ownerId = userContext.GetUserId();
            var query = new GetProjectsByOwnerIdQuery(ownerId);
            var result = await sender.Send(query);
            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetAllByOwner");
        
    }
}

