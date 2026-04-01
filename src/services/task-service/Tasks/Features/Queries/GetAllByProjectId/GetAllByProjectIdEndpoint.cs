
namespace task_service.Tasks.Features.Queries.GetAllByProjectId;


public class GetAllByProjectIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/project_id={projectId:guid}",
            async (Guid projectId, ISender sender, [FromServices]ClaimsPrincipal claims) =>
            {
                var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

                var result = await sender.Send(
                    new GetAllByProjectIdQuery(userId, projectId));

                return Results.Ok(result);

            }).RequireAuthorization()
            .WithName("GetAllProjectId");
    }
}