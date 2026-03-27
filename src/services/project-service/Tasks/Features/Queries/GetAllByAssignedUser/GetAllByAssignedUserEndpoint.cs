namespace project_service.Tasks.Features.Queries.GetAllByAssignedUser;

public class GetAllByAssignedUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/assigned/me", async ([FromServices]ISender sender, ClaimsPrincipal claims) =>
        {
            var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            var result = await sender.Send(new GetAllByAssignedUserQuery(userId));
            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
