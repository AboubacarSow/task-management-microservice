namespace project_service.Tasks.Features.Queries.GetTasksByOwnerId;


public class GetTasksByOwnerIdEndpoint : ICarterModule
{
     public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/me", async (ISender sender, IUserContext userContext) =>
        {
            var ownerId = userContext.GetUserId();
            var result = await sender.Send(new GetTasksByOwnerIdQuery(ownerId));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("AllTasksByOwner");
    }
}