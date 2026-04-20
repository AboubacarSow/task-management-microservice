using shared.Utilities;

namespace task_service.Tasks.Features.Queries.GetTasksByOwnerId;


public class GetTasksByOwnerIdEndpoint : ICarterModule
{
     public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/me", async (ISender sender,
            [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();

            var result = await sender.Send(new GetTasksByOwnerIdQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("AllTasksByOwner");
    }
}