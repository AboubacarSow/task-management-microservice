namespace project_service.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/tasks/{id:guid}", async (Guid id, [FromServices]ISender sender, ClaimsPrincipal claims) =>
        {
                var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            var result = await sender.Send(new GetTaskByIdQuery(userId,id));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("GetTaskById");
    }
}