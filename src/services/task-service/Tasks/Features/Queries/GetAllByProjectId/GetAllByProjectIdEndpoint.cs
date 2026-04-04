
namespace task_service.Tasks.Features.Queries.GetAllByProjectId;


public class GetAllByProjectIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/project_id={projectId:guid}",
            async (Guid projectId, ISender sender,
            ClaimsPrincipal claims) =>
            {

                var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(currentUserId, out var userId))
                    return Results.BadRequest("Invalid user id");


                var result = await sender.Send(
                    new GetAllByProjectIdQuery(userId, projectId));

                return Results.Ok(result);

            }).RequireAuthorization()
            .WithName("GetAllProjectId");
    }
}