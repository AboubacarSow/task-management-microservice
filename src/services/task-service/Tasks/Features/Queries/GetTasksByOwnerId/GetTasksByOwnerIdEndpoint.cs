namespace task_service.Tasks.Features.Queries.GetTasksByOwnerId;


public class GetTasksByOwnerIdEndpoint : ICarterModule
{
     public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/me", async (ISender sender, ClaimsPrincipal claims) =>
        {
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");

            var result = await sender.Send(new GetTasksByOwnerIdQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("AllTasksByOwner");
    }
}