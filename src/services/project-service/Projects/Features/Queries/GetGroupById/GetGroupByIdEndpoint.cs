namespace project_service.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/group",
            async ([FromRoute]Guid projectId, [FromServices]ISender sender, 
            [FromServices]IUserContext userContext) =>
            {
                var currentUserId= userContext.GetUserId(); 

                var result = await sender.Send(new GetGroupByIdQuery(currentUserId,projectId));


                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetGroup");
    }
}