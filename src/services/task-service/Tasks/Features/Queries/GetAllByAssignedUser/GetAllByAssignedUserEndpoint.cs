


namespace task_service.Tasks.Features.Queries.GetAllByAssignedUser;

public class GetAllByAssignedUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/assigned/me", async ([FromServices]ISender sender, 
        ClaimsPrincipal claims) =>
        {
        
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");
            var result = await sender.Send(new GetAllByAssignedUserQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
