namespace project_service.Tasks.Features.Queries.GetTasksByOwnerId;


public class GetTasksByOwnerIdEndpoint : ICarterModule
{
     public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/me", async (ISender sender, ClaimsPrincipal claims) =>
        {
                var ownerId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            var result = await sender.Send(new GetTasksByOwnerIdQuery(ownerId));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("AllTasksByOwner");
    }
}