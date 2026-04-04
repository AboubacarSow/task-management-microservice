namespace project_service.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/me", async (
            [FromServices]ISender sender,
            ClaimsPrincipal claims) =>
        {
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");
            var query = new GetProjectsByOwnerIdQuery(userId);
            var result = await sender.Send(query);
            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetAllByOwner");
        
    }
}

