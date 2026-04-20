


using shared.Utilities;

namespace task_service.Tasks.Features.Queries.GetAllByAssignedUser;

public class GetAllByAssignedUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/assigned/me", async ([FromServices]ISender sender,
           [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();
            var result = await sender.Send(new GetAllByAssignedUserQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
