namespace project_service.Tasks.Features.Queries.GetAllByAssignedUser;

public class GetAllByAssignedUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/assigned/me", async (ISender sender, IUserContext userContext) =>
        {
            var userId = userContext.GetUserId();
            var result = await sender.Send(new GetAllByAssignedUserQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
