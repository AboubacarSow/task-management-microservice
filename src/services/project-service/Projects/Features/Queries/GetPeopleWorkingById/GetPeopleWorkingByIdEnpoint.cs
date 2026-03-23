namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/people",
        async (Guid projectId, ISender sender, IUserContext userContext) =>
        {

            var currentUserId = userContext.GetUserId();
            var result = await sender.Send(new GetPeopleWorkingByIdQuery(currentUserId,projectId));


            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetPeople");
    }
}
