using shared.Utilities;

namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/people",
        async ([FromRoute]Guid projectId, [FromServices]ISender sender,
          [FromServices] IUserContext claims) =>
        {
            var userId = claims.GetUserId();
            var result = await sender.Send(new GetPeopleWorkingByIdQuery(userId,projectId));


            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetPeople");
    }
}
