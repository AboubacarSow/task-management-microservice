namespace project_service.Tasks.Features.Queries.GetAllByProjectId;


public class GetAllByProjectIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/project_id={projectId:guid}",
            async (Guid projectId, ISender sender, IUserContext userContext) =>
            {
                var userId = userContext.GetUserId();

                var result = await sender.Send(
                    new GetAllByProjectIdQuery(userId, projectId));

                return Results.Ok(result);

            }).RequireAuthorization()
            .WithName("GetAllProjectId");
    }
}