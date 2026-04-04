namespace project_service.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/people",
        async ([FromRoute]Guid projectId, [FromServices]ISender sender, 
        ClaimsPrincipal claims) =>
        {

           var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id"); 
            var result = await sender.Send(new GetPeopleWorkingByIdQuery(userId,projectId));


            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetPeople");
    }
}
