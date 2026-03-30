namespace project_service.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/me", async (
            [FromServices]ISender sender,
            [FromServices]ClaimsPrincipal claims) =>
        {
            var ownerId =Guid.Parse(claims.FindFirst("sub")?.Value!); 
            var query = new GetProjectsByOwnerIdQuery(ownerId);
            var result = await sender.Send(query);
            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetAllByOwner");
        
    }
}

