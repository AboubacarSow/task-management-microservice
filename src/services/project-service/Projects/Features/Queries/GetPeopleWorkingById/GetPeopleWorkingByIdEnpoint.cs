namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/people",
        async ([FromRoute]Guid projectId, [FromServices]ISender sender, 
        [FromServices]ClaimsPrincipal claims) =>
        {

            var currentUserId =Guid.Parse(claims.FindFirst("sub")?.Value!); 
            var result = await sender.Send(new GetPeopleWorkingByIdQuery(currentUserId,projectId));


            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetPeople");
    }
}
