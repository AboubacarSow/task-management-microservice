using shared.Utilities;

namespace project_service.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{id:guid}", 
                async ([FromRoute]Guid id, [FromServices]ISender sender,
                [FromServices] IUserContext claims) =>
        {
             var userId =claims.GetUserId();
            var query = new GetProjectByIdQuery(userId,id);
            var result = await sender.Send(query);

            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetById");
    }
}
